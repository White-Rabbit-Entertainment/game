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

  void Awake() {
    if (!tutorialTask) {
      taskManager = GameObject.Find("/TaskManager").GetComponent<TaskManager>();
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
    isAssigned = false;
    PlayableCharacter me =  NetworkManager.instance.GetMe();
    me.taskNotificationUI.SetNotification(true);
    if (parent !=  null) {
      parent.View.RPC("SetTaskGlowRPC", RpcTarget.All);
    }
    View.RPC("SetTaskGlowRPC", RpcTarget.All);;
    if (!tutorialTask) {
      taskManager.CheckAllTasksCompleted();
    }
    GetComponent<Interactable>().DisableTarget();
    if (isUndoable && NetworkManager.instance.GetMe() is Traitor) {
      EnableTarget();
    }
  }
   
  // Complete taks and consume all the requirements  eg pocketables
  public void CompleteAndConsume(Character character) {
    Complete();
    foreach(Task requirement in requirements) {
      requirement.TaskInteractable.OnParentTaskComplete(character);
    }
  }
  
  public void Complete() {
    if (tutorialTask) {
      CompleteRPC();
    } else {
      View.RPC("CompleteRPC", RpcTarget.All);
    }
  }

  public void ManualComplete() {
    Complete();
    foreach (Task requirement in requirements) {
      requirement.ManualComplete();
    }
    if (TaskInteractable is Stealable) {
      View.TransferOwnership(PhotonNetwork.LocalPlayer);
      TaskInteractable.transform.position = ((Stealable)TaskInteractable).destination.transform.position;
    }
    TaskInteractable.PlayItemAnimation();
  }
  
  [PunRPC]
  public void UncompleteRPC() {
    isCompleted = false;
    isAssigned = false;
    NetworkManager.instance.GetMe().taskNotificationUI.SetNotification(false);
    if (parent != null) {
      parent.View.RPC("SetTaskGlowRPC", RpcTarget.All);
    }
    View.RPC("SetTaskGlowRPC", RpcTarget.All);
    
    foreach(Task requirement in requirements) {
      requirement.TaskInteractable.OnParentTaskUncomplete();
    }
    if (NetworkManager.instance.GetMe() is Traitor) {
      DisableTarget();
    }
  }

  public void Uncomplete() {
    View.RPC("UncompleteRPC", RpcTarget.All);
  }

  public bool IsRequired() {
    return !IsMasterTask() && !parent.isCompleted;
  }

  public void Assign(PlayableCharacter character) {
    Debug.Log($"Assign task to {character.Owner.NickName}");
    //Master calls assignToCharacter first to ensure it is done before anyone else
    AssignToCharacter(character);
    //Then we call AssignToCharacter on all other players
    View.RPC("AssignRPC", RpcTarget.Others, character.View.ViewID);
  }

  [PunRPC]
  public void AssignRPC(int assignedCharacterViewId) {
    PlayableCharacter character = PhotonView.Find(assignedCharacterViewId).GetComponent<PlayableCharacter>();
    AssignToCharacter(character);
  }
  
  public void AssignToCharacter(PlayableCharacter character) {
    character.assignedTask = this;
    isAssigned = true;
    if (character.IsMe()) {
      EnableTarget();
      character.contextTaskUI.SetTask(this);
      taskManager.requested = false;
    }
  }

   public void Unassign() {
   View.RPC("UnassignRPC", RpcTarget.All);
  }

  [PunRPC]
  public void UnassignRPC() {
    isAssigned = false;
  }

  public void EnableTarget() {
    Interactable interactable = GetComponent<Interactable>();
    interactable.EnableTarget();
    Debug.Log("Task Enabled");
  }

  public void DisableTarget() {
    Interactable interactable = GetComponent<Interactable>();
    interactable.DisableTarget();
  }
  
}
