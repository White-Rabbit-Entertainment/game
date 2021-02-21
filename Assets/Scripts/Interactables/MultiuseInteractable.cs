using UnityEngine;
using Photon.Pun;

class MultiuseInteractable : Interactable, Taskable {

  public string description;
  // public Animation animation; 

  public void OnClick() {
    // Run the animation
    Debug.Log("Doing an animation");
    // Animation
    
    Task task = GetComponent<Task>();
    if (task != null) {
      task.Complete();
    }
  }
  
  [PunRPC]
  public void AddTask() {
    Task task = gameObject.AddComponent<Task>() as Task;
    task.description = description; 
  }
}
