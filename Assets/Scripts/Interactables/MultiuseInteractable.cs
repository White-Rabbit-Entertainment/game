using UnityEngine;
using Photon.Pun;

class MultiuseInteractable : Interactable, Taskable {

  public string description;
  public Material material;
  // public Animation animation; 

  public void OnClick() {
    // Run the animation
    Debug.Log("Doing an animation");
    // Animation
    Task task = GetComponent<Task>();
    if (task != null && NetworkManager.instance.LocalPlayerPropertyIs<string>("Team", "Robber")) {
      GetComponent<PhotonView>().RPC("Complete", RpcTarget.All);
    }
  }

  [PunRPC]
  private void Complete() {
    Task task = GetComponent<Task>();
    task.Complete();
  }
  
  [PunRPC]
  public void AddTask() {
    if (NetworkManager.instance.LocalPlayerPropertyIs("Team", "Robber")) {
			  gameObject.GetComponent<MeshRenderer>().material = material;
		  }
    Task task = gameObject.AddComponent<Task>() as Task;
    task.description = description;
  }
}
