using Photon.Pun;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Task : MonoBehaviour {
  public bool isCompleted = false;
  public string description;
  public TaskManager taskManager;
  public bool isUndoable = true;

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
    if (parent !=  null) {
      parent.View.RPC("SetTaskGlow", RpcTarget.All);
    }
    View.RPC("SetTaskGlow", RpcTarget.All);
    taskManager.CheckAllTasksCompleted();
  }
    
  public void Complete() {
    View.RPC("CompleteRPC", RpcTarget.All);
  }
  
  [PunRPC]
  public void UncompleteRPC() {
    isCompleted = false;
    if (parent !=  null) {
      parent.View.RPC("SetTaskGlow", RpcTarget.All);
    }
    View.RPC("SetTaskGlow", RpcTarget.All);
  }

  public void Uncomplete() {
    View.RPC("UncompleteRPC", RpcTarget.All);
  }

  public bool IsRequired() {
    return !IsMasterTask() && !parent.isCompleted;
  }
  
}
