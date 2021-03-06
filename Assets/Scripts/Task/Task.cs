using Photon.Pun;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Task : MonoBehaviour {
  public bool isCompleted = false;
  public string description;

  // This is the list of requirements that must be completed before this task
  // can be completed 
  public List<Task> requirements = new List<Task>();
 
  // This is the task which depends on this being completed first. If this is
  // null then the task is a "master" task.
  public Task parent;


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
  public void Complete() {
    isCompleted = true;
  }
}
