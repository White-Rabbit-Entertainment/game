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
  public void CompleteRPC(bool isManual) {
    isCompleted = true;
    isAssigned = false;
    PlayableCharacter me =  NetworkManager.instance.GetMe();
    //DisableUndoneMarker();
    me.taskNotificationUI.SetNotification(true);
    if (parent !=  null) {
      parent.View.RPC("SetTaskGlowRPC", RpcTarget.All);
    }
    View.RPC("SetTaskGlowRPC", RpcTarget.All);;
    if (!tutorialTask) {
      taskManager.CheckAllTasksCompleted();
    }
    GetComponent<Interactable>().DisableTarget();
    if (isUndoable && me is Traitor) {
      EnableTarget();
    }
    foreach(Task requirement in requirements) {
      requirement.TaskInteractable.OnParentTaskComplete(me);
    }
    // When you complete a task if you are a loyal you want a new one
    if (!isManual) {
      if (me is Loyal && (me.assignedSubTask == null || me.assignedSubTask.isCompleted)) {
        if (me.assignedMasterTask == null || me.assignedMasterTask.isCompleted) {
          Debug.Log($"Requesting new task after non manual complete of {gameObject}");
          taskManager.RequestNewTask();
        } else {
          me.assignedMasterTask.AssignSubTaskToCharacter(me);
        }
      }
    } else {
      foreach (Task requirement in requirements) {
        requirement.CompleteRPC(true);
      }
      if (TaskInteractable is Stealable) {
        View.TransferOwnership(PhotonNetwork.LocalPlayer);
        Stealable stealable = ((Stealable)TaskInteractable);
        // When manually complete a stealable you dont want to also register
        // the collision with the endzone (and recomplete). Therefore we
        // disable the next collision.
        stealable.ignoreNextCollision = true;
        // Move stealable to endzone inorder to complete
        TaskInteractable.transform.position = stealable.destination.transform.position;
      }
      if (PhotonNetwork.IsMasterClient) {
        TaskInteractable.PlayItemAnimation();
      }
    }
  }
  
  public void Complete(bool isManual = false) {
    Debug.Log("Completing task");
    if (tutorialTask) {
      CompleteRPC(isManual);
    } else {
      View.RPC("CompleteRPC", RpcTarget.All, isManual);
    }
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
    } else {
      //EnableUndoneMarker();
    }
  }

  public void Uncomplete() {
    View.RPC("UncompleteRPC", RpcTarget.All);
  }

  public bool IsRequired() {
    return !IsMasterTask() && !parent.isCompleted;
  }

  public void AssignTask(PlayableCharacter character) {
    Debug.Log($"Assign task to {character.Owner.NickName}");
    //Master calls assignToCharacter first to ensure it is done before anyone else
    AssignTaskToCharacter(character);
    //Then we call AssignToCharacter on all other players
    View.RPC("AssignTaskToCharacterRPC", RpcTarget.Others, character.View.ViewID);
  }

  [PunRPC]
  public void AssignTaskToCharacterRPC(int assignedCharacterViewId) {
    PlayableCharacter character = PhotonView.Find(assignedCharacterViewId).GetComponent<PlayableCharacter>();
    AssignTaskToCharacter(character);
  }
  
  private void AssignTaskToCharacter(PlayableCharacter character) {
    Debug.Log($"Assining master taks: {this}");
    character.assignedMasterTask = this;
    isAssigned = true;
    if (character.IsMe()) {
      Debug.Log($"Assining master taks to me: {this}");
      AssignSubTaskToCharacter(character);
    }
  }

  public void AssignSubTaskToCharacter(PlayableCharacter character) {
    Debug.Log($"Giving my self a master task: {FindIncompleteChild(this)}");
    Task subTask = FindIncompleteChild(this);
    character.assignedSubTask = subTask;
    character.contextTaskUI.SetTask(subTask);
    subTask.EnableTarget();
  }

  private Task FindIncompleteChild(Task task) {
    if (task.AllChildrenCompleted()) {
      return task;
    } else {
      foreach (Task child in task.requirements) {
        if (!child.isCompleted) {
          return FindIncompleteChild(child);
        }
      }
    }

    // This should be impossible
    Debug.Log("This should be impossible");
    return null;
  }

  public void Unassign() {
    View.RPC("UnassignRPC", RpcTarget.All);
  }

  [PunRPC]
  public void UnassignRPC() {
    isAssigned = false;
  }

  public void EnableTarget() {
    GetComponent<Interactable>().EnableTarget();
  }

  public void DisableTarget() {
    GetComponent<Interactable>().DisableTarget();
  }

  public void EnableUndoneMarker() {
    GetComponent<Interactable>().EnableUndoneMarker();
  }

  public void DisableUndoneMarker() {
    GetComponent<Interactable>().DisableUndoneMarker();
  }
  
}
