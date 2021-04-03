using Photon.Pun;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[DisallowMultipleComponent]
[RequireComponent(typeof(Interactable))]
public class Task : MonoBehaviour {
  public bool isCompleted = false;
  public string description;
  public TaskManager taskManager;
  public bool isUndoable = true;

  public Timer timer = Timer.None;

  private Interactable TaskInteractable {
    get { return GetComponent<Interactable>(); }
  }
  // This is the list of requirements that must be completed before this task
  // can be completed 
  public List<Task> requirements = new List<Task>();
 
  // This is the task which depends on this being completed first. If this is
  // null then the task is a "master" task.
  public Task parent;

  public bool tutorialTask = false;

  public PhotonView View {
    get { return GetComponent<PhotonView>(); }
  }

  void Start() {
    if (!tutorialTask) {
      taskManager = GameObject.Find("/TaskManager").GetComponent<TaskManager>();
      taskManager.AddTask(this);
    }
  }

  // Returns if the task is a master task, i.e. no tasks depend on this task
  public bool IsMasterTask() {
    return parent == null;
  }

  // Returns true if all the requirements of this task have been completed. If this
  // is the case then this task can now be attempted. If false the requirement
  // tasks must be completed first.
  public bool AllChildrenCompleted() {
    foreach(Task requirement in requirements) {
      if (!requirement.isCompleted) {
        return false;
      }
      // If the requirement is a pocketable
      if (requirement.TaskInteractable is Pocketable) {
        // And you do not have that pocketable
        if (!NetworkManager.instance.GetMe().HasItem(requirement.TaskInteractable)) {
          // Then return false
          return false;
        }
      }
    }
    return true;
  }

  [PunRPC]
  public void CompleteRPC() {
    isCompleted = true;
    if (parent != null) {
      parent.View.RPC("SetTaskGlowRPC", RpcTarget.All);
    }
    View.RPC("SetTaskGlowRPC", RpcTarget.All);
    if (!tutorialTask) {
      taskManager.CheckAllTasksCompleted();
    }
    GetComponent<Interactable>().DisableTarget();
  }
   
  // Complete taks and consume all the requirements  eg pocketables
  public void CompleteAndConsume(Character character) {
    foreach(Task requirement in requirements) {
      requirement.TaskInteractable.OnTaskComplete(character);
    }
    Complete();
  }
  
  public void Complete() {
    if (tutorialTask) {
      CompleteRPC();
    } else {
      View.RPC("CompleteRPC", RpcTarget.All);
    }
  }
  
  [PunRPC]
  public void UncompleteRPC() {
    isCompleted = false;
    if (parent !=  null) {
      parent.View.RPC("SetTaskGlowRPC", RpcTarget.All);
    }
    if (IsMasterTask() && AllChildrenCompleted()) {
      foreach(Task task in requirements) {
        if (NetworkManager.instance.GetMe().HasItem(task.GetComponent<Interactable>())) {
          GetComponent<Interactable>().EnableTarget();
        }
      }
    }
    View.RPC("SetTaskGlowRPC", RpcTarget.All);
    
    foreach(Task requirement in requirements) {
      requirement.TaskInteractable.OnTaskUncomplete();
    }
    View.RPC("SetTaskGlowRPC", RpcTarget.All);
  }

  public void Uncomplete() {
    View.RPC("UncompleteRPC", RpcTarget.All);
  }

  public bool IsRequired() {
    return !IsMasterTask() && !parent.isCompleted;
  }
  
}
