using UnityEngine;
using Photon.Pun;

class MultiuseInteractable : Interactable, Taskable {

  public void OnClick() {
    // Run the animation
    Debug.Log("Doing an animation");
    
    Task task = GetComponent<Task>();
    if (task != null) {
      task.Complete();
    }
  }
  
  [PunRPC]
  public void AddTask() {
    Task task = GetComponent<Task>();
  }
}
