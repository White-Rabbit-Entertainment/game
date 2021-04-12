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
  public GameObject taskables;
  public List<Task> tasks;
  public GameSceneManager gameSceneManager;
  public TaskCompletionUI taskCompletionUI;

  private bool requested = false;

  void Start() {
    tasks = new List<Task>();
    InvokeRepeating("UpdateTaskBar", 5f, 5f);
  }

  void Update() {
    if (PhotonNetwork.LocalPlayer.IsMasterClient && 
        NetworkManager.instance.CheckAllPlayers<string>("CurrentScene", "GameScene") && 
        NetworkManager.instance.AllCharactersSpawned() && 
        !NetworkManager.instance.RoomPropertyIs<bool>("TasksSet", true)
    ) {
      SetTasks();
    }

    // Set inital task
    if (
        !requested
      && NetworkManager.instance.RoomPropertyIs("TasksSet", true) 
      && NetworkManager.instance.RoomPropertyIs<int>("NumberOfTasksSet", tasks.Count) 
    ) {
      requested = true;
      PlayableCharacter character = NetworkManager.instance.GetMe();
      if (character is Loyal) {
        if (character.assignedMasterTask == null || character.assignedMasterTask.isCompleted) {
          RequestNewTask();
        } else {
          character.assignedMasterTask.AssignSubTaskToCharacter(character);
        }
      }
    }
  }

  public void UpdateTaskBar() {
    Debug.Log("Updating task bar");
    taskCompletionUI.UpdateBar();
  }

  public void AddTask(Task task) {
    tasks.Add(task);
  }

  void SetTasks() {
    // Get the number of tasks of each type which should be created
    int expectedNumberOfTasks = 0;
    int numberOfTasks = NetworkManager.instance.GetRoomProperty<int>("NumberOfTasks");
    int numberOfTasksInitiallyCompleted = NetworkManager.instance.GetRoomProperty<int>("NumberOfTasksInitiallyCompleted");
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
        if (possibleMasterTaskables[i].GetComponent<Interactable>().HasSoftRequirements()) {
            int numberOfSubTasks = UnityEngine.Random.Range(0, 2);
            if (numberOfSubTasks > 0) {
                  possibleMasterTaskables[i].GetComponent<Interactable>().PickHardRequirements(possibleTaskables);
            }
        }
        expectedNumberOfTasks++;
        PhotonView view = possibleMasterTaskables[i].GetComponent<PhotonView>();
        if (i < numberOfTasksInitiallyCompleted) {
          view.RPC("AddCompletedTaskRPC", RpcTarget.All);
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

  public int NumberOfTasksCompleted() {
    int count = 0;
    foreach(Task task in tasks) {
      if (task.isCompleted) {
        count++;
      }
    }
    return count;
  }

  public Task FindUncompleteTask() {
    foreach(Task task in tasks) {
      if (!task.isCompleted) {
        return task;
      }
    }
    return null;
  }

  public Task FindUnassignedTask() {
    foreach(Task task in tasks) {
      if (!task.isAssigned && !task.isCompleted) {
        return task;
      }
    }
    return null;
  }

  public void RequestNewTask() {
    GetComponent<PhotonView>().RPC("AssignTask", PhotonNetwork.MasterClient, NetworkManager.instance.GetMe().View.ViewID);
  }

  [PunRPC]
  public void AssignTask(int requestingPlayerViewId) {
    PlayableCharacter taskRequester = PhotonView.Find(requestingPlayerViewId).GetComponent<PlayableCharacter>();
    Task nextTask = null;
    if (nextTask == null) nextTask = FindUnassignedTask();
    if (nextTask == null) nextTask = FindUncompleteTask();
    if (nextTask != null) {
      nextTask.AssignTask(taskRequester);
    }
  }

  /// <summary> Return if all Loyal tasks have been completed. </summary> 
  public void CheckAllTasksCompleted() {
    if (NumberOfTasksCompleted() == tasks.Count && NetworkManager.instance.RoomPropertyIs<bool>("TasksSet", true)) {
      gameSceneManager.EndGame(Team.Loyal);
    }
  }
}
