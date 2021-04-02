using Photon.Pun;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[DisallowMultipleComponent]
[RequireComponent(typeof(Interactable))]
public class Task : MonoBehaviour {
  public bool isCompleted = false;

  public bool isAssigned = false;
  public string description;
  public TaskManager taskManager;
  public bool isUndoable = true;

  public Timer timer = Timer.None;

  // This is the list of requirements that must be completed before this task
  // can be completed 
  public List<Task> requirements = new List<Task>();
 
  // This is the task which depends on this being completed first. If this is
  // null then the task is a "master" task.
  public Task parent;

  public PhotonView View {
    get { return GetComponent<PhotonView>(); }
  }

  void Start() {
    taskManager = GameObject.Find("/TaskManager").GetComponent<TaskManager>();
    taskManager.AddTask(this);
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
    }
    return true;
  }

  [PunRPC]
  public void CompleteRPC() {
    isCompleted = true;
    PlayableCharacter me =  NetworkManager.instance.GetMe();
    me.taskNotificationUI.SetNotification(true);
    if (parent !=  null) {
      parent.View.RPC("SetTaskGlowRPC", RpcTarget.All);
    }
    View.RPC("SetTaskGlowRPC", RpcTarget.All);
    taskManager.CheckAllTasksCompleted();
    GetComponent<Interactable>().DisableTarget();
  }
    
  public void Complete() {
    PlayableCharacter me =  NetworkManager.instance.GetMe();
    me.assignedTask = null;
    View.RPC("CompleteRPC", RpcTarget.All);
  }
  
  [PunRPC]
  public void UncompleteRPC() {
    isCompleted = false;
    isAssigned = false;
    NetworkManager.instance.GetMe().taskNotificationUI.SetNotification(false);
    if (parent != null) {
      parent.View.RPC("SetTaskGlowRPC", RpcTarget.All);
    }
    if (IsMasterTask() && AllChildrenCompleted()) {
      foreach(Task task in requirements) {
        if (NetworkManager.instance.GetMe().HasItem(task.GetComponent<Interactable>())) {
          GetComponent<Interactable>().EnabledTarget();
        }
      }
    }
    View.RPC("SetTaskGlowRPC", RpcTarget.All);
  }

  public void Uncomplete() {
    View.RPC("UncompleteRPC", RpcTarget.All);
  }

  public bool IsRequired() {
    return !IsMasterTask() && !parent.isCompleted;
  }

  public void Assign() {
    View.RPC("AssignRPC", RpcTarget.All);
  }

  [PunRPC]
  public void AssignRPC() {
    isAssigned = true;
  }

   public void Unassign() {
   View.RPC("UnassignRPC", RpcTarget.All);
  }

  [PunRPC]
  public void UnassignRPC() {
    isAssigned = false;
  }

  public void EnabledTarget() {
    Interactable interactable = GetComponent<Interactable>();
    interactable.EnabledTarget();
  }

  public void DisableTarget() {
    Interactable interactable = GetComponent<Interactable>();
    interactable.DisableTarget();
  }
  
}
