using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
//using UnityEngine.JSONSerializeModule;

/// <summary><c>GameManager</c> is the brain of the game. It contains most of
/// the important logic such as GameOver logic. The game manager is a singleton
/// (meaning one static instance of it exists). It is created in the first
/// (multiplayer menu) scene. It can be referenced at any time with <code>
/// GameManager.instance </code></summary> 
public class TaskManager : MonoBehaviourPun {
  
  // GameObject which holds all items that can be assigned tasks 
  public GameObject taskables;
  
  // The list of all master tasks
  public List<Task> tasks;

  public GameSceneManager gameSceneManager;

  // UI
  public TaskCompletionUI taskCompletionUI;

  // Has the local player requested a task and not yet received one (prevents
  // mulitple requests at once).
  private bool requested = false;

  // Are all the tasks completed (prevents multiple endgames)
  private bool allTasksCompleted = false;

  void Start() {
    tasks = new List<Task>();

    // Every 5 seconds update the task bar ui
    InvokeRepeating("UpdateTaskBar", 5f, 5f);
  }

  void Update() {
    // If all players are in the scene and spawned then assign tasks
    if (PhotonNetwork.LocalPlayer.IsMasterClient && 
        NetworkManager.instance.CheckAllPlayers<string>("CurrentScene", "GameScene") && 
        NetworkManager.instance.AllCharactersSpawned() && 
        !NetworkManager.instance.RoomPropertyIs<bool>("TasksSet", true)
    ) {
      SetTasks();
    }

    // Set inital task for player
    if (
        !requested
      && NetworkManager.instance.RoomPropertyIs("TasksSet", true) 
    ) {
      requested = true;
      PlayableCharacter character = NetworkManager.instance.GetMe();
      if (character is Loyal) {
        Debug.Log("Requesting new task!");
        RequestNewTask();
      }
    }
  }

  public void UpdateTaskBar() {
    taskCompletionUI.UpdateBar();
  }

  public void AddTask(Task task) {
    tasks.Add(task);
  }

  // Sets the tasks at the start of the game. This selects interactables from
  // the taskables game object and assigns tasks to them as well as their
  // requirements. Check Task class for more details.
  void SetTasks() {
    // Get the number of tasks of each type which should be created
    int expectedNumberOfTasks = 0;
    int numberOfTasks = NetworkManager.instance.GetRoomProperty<int>("NumberOfTasks");
    int numberOfTasksInitiallyCompleted = NetworkManager.instance.GetRoomProperty<int>("NumberOfTasksInitiallyCompleted");

    int numberOfCompletedTasks = 0;
    // Get all possible items to assign tasks to in the environment 
    // We split this so we can assign the correct number of stealing
    // tasks.
    List<Transform> possibleMasterTaskables = new List<Transform>();
    List<Transform> possibleTaskables = new List<Transform>();

    // Creates a list of all item which can be assigned a task and
    // seperate list for those which can be master tasks
    foreach(Transform transform in taskables.transform) {
        possibleTaskables.Add(transform);
        if (transform.GetComponent<Interactable>().canBeMasterTask) {
            possibleMasterTaskables.Add(transform);
        }
    }

    // Shuffle those lists randomly 
    possibleMasterTaskables.Shuffle();

    // Assign the first few items a Task
    for (int i = 0; i < numberOfTasks; i++) {
        Interactable interactable = possibleMasterTaskables[i].GetComponent<Interactable>();

        // If it has soft requirements
        if (interactable.HasSoftRequirements()) {
            // Then assign a soft requirement as a requirement with a
            // probability of softRequirementProbability 
            float giveSubTaskWeight = UnityEngine.Random.Range(0f, 1f); 
            if (giveSubTaskWeight <= interactable.softRequirementProbability) {
                // Picks a soft requirement and assigns it as hard requirement
                interactable.PickHardRequirements(possibleTaskables);
            }
        }
        // Counts number of assigned tasks
        expectedNumberOfTasks++;
        PhotonView view = interactable.GetComponent<PhotonView>();

        // Manually completes some tasks so the traitor has a clear objective
        // at the start of the game.
        if (numberOfCompletedTasks < numberOfTasksInitiallyCompleted) {
            if (!(interactable is Stealable)) {
              view.RPC("AddCompletedTaskRPC", RpcTarget.All);
              numberOfCompletedTasks++;
            }
        } else {
            view.RPC("AddTaskRPC", RpcTarget.All);
        }
    }

    // Say that we have finished the work of setting up tasks (used for
    // knowing everything is loaded).
    NetworkManager.instance.SetRoomProperty("TasksSet", true);
    NetworkManager.instance.SetRoomProperty("NumberOfTasksSet", expectedNumberOfTasks);
  }

  public List<Task> GetTasks() {
    return tasks;
  }

  // Return number of master tasks which have been completed
  public int NumberOfTasksCompleted() {
    int count = 0;
    foreach(Task task in tasks) {
      if (task.isCompleted) {
        count++;
      }
    }
    return count;
  }

  // Find a task in the scene which hasnt yet been completed
  public Task FindUncompleteTask() {
    foreach(Task task in tasks) {
      if (!task.isCompleted) {
        return task;
      }
    }
    return null;
  }

  // Find a task in the scene which hasnt yet been completed and also hasnt yet
  // been assigned
  public Task FindUnassignedTask() {
    foreach(Task task in tasks) {
      if (!task.isAssigned && !task.isCompleted) {
        return task;
      }
    }
    return null;
  }

  // Used by local player to request a new task from the master client. Master
  // client is used to ensure syncrhoniation between clients (so noone gets the
  // same task once).
  public void RequestNewTask() {
    GetComponent<PhotonView>().RPC("AssignTask", PhotonNetwork.MasterClient, NetworkManager.instance.GetMe().View.ViewID);
  }

  // Master client assigns task to the requesting player. It first looks for an
  // unassinged task, then if none available it then picks an uncompleted task
  // (but assinged to another player).
  [PunRPC]
  public void AssignTask(int requestingPlayerViewId) {
    PlayableCharacter taskRequester = PhotonView.Find(requestingPlayerViewId).GetComponent<PlayableCharacter>();
    Task nextTask = null;
    // Looks for unassigned task
    if (nextTask == null) nextTask = FindUnassignedTask();
    // Looks for a completed task if unassinged task not found
    if (nextTask == null) nextTask = FindUncompleteTask();
    // If found a suitable task to assign then assings it to player
    if (nextTask != null) {
      nextTask.AssignTask(taskRequester);
    }
  }

  /// Return if all master tasks have been completed. If this is true the game
  /// should be ended and the loyals will win.
  public void CheckAllTasksCompleted() {
    if (NumberOfTasksCompleted() == tasks.Count && NetworkManager.instance.RoomPropertyIs<bool>("TasksSet", true) && !allTasksCompleted) {
      allTasksCompleted = true;
      gameSceneManager.EndGame(Team.Loyal);
    }
  }
}
