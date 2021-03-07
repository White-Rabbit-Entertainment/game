using Photon.Pun;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Task : MonoBehaviour {
  public bool isCompleted = false;
  public string description;
  public Task parent;
  public List<Task> children;

  public bool IsMasterTask() {
    return parent == null;
  }

  public bool AllChildrenCompleted() {
    foreach(Task child in children) {
      if (!child.isCompleted) {
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
