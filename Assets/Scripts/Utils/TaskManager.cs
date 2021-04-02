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

  void Start() {
    tasks = new List<Task>();
  }

  void Update() {
    if (PhotonNetwork.LocalPlayer.IsMasterClient && 
        NetworkManager.instance.CheckAllPlayers<string>("CurrentScene", "GameScene") && 
        NetworkManager.instance.AllCharactersSpawned() && 
        !NetworkManager.instance.RoomPropertyIs<bool>("TasksSet", true)
    ) {
      SetTasks();
    }

    if (NetworkManager.instance.GetMe().assignedTask == null) {
       Debug.Log("New Task!");
       AssignTask();
       Debug.Log(NetworkManager.instance.GetMe().assignedTask.description);
       NetworkManager.instance.GetMe().assignedTask.EnabledTarget();
     }
  }

  public void AddTask(Task task) {
    tasks.Add(task);
  }

  void SetTasks() {
    // Get the number of tasks of each type which should be created
    int numberOfTasks = NetworkManager.instance.GetRoomProperty<int>("NumberOfTasks");
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
        PhotonView view = possibleMasterTaskables[i].GetComponent<PhotonView>();
        view.RPC("AddTaskRPC", RpcTarget.All);
    }
    // Say that we have finished the work of setting up tasks (used for
    // knowing everything is loaded).
    NetworkManager.instance.SetRoomProperty("TasksSet", true);
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
        Debug.Log("Found");
        return task;
      }
    }
    return null;
  }

  public Task FindUnassignedTask() {
    Debug.Log("Running");
    foreach(Task task in tasks) {
      if (!task.isAssigned) {
        Debug.Log("Assigned");
        return task;
      }
    }
    return null;
  } 

  public void AssignTask() {
    Task nextTask = FindUnassignedTask();
    if (nextTask == null) nextTask = FindUncompleteTask();
    // if (nextTask != null) {
      nextTask.Assign();
      NetworkManager.instance.GetMe().assignedTask = nextTask;
    // }
  }

  /// <summary> Return if all Loyal tasks have been completed. </summary> 
  public void CheckAllTasksCompleted() {
    if (NumberOfTasksCompleted() == tasks.Count) {
      gameSceneManager.EndGame(Team.Loyal);
    }
  }
}
