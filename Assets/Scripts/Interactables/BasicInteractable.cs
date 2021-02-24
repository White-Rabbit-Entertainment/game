using UnityEngine;
using Photon.Pun;

class BasicInteractable : Interactable {

  public Material material;
  public Team team;
  public bool singleUse;
  public Animation animation; 

  public override void PrimaryInteraction() {
    // Run the animation
    Debug.Log("Doing an animation");
    // Animation
    Task task = GetComponent<Task>();
    if (task != null && NetworkManager.instance.LocalPlayerPropertyIs<string>("Team", "Robber")) {
      GetComponent<PhotonView>().RPC("CompleteTask", RpcTarget.All);
    }
    if (animation != null) {
      animation.Play();
    }
    if (singleUse) Destroy(this);
  }

  public override bool CanInteract() {
    if (team == Team.Both) return true;
    if (NetworkManager.instance.LocalPlayerPropertyIs<string>("Team", "Seeker") && team == Team.Seeker) return true;
    if (NetworkManager.instance.LocalPlayerPropertyIs<string>("Team", "Robber") && team == Team.Robber) return true;
    return false;
  }

  [PunRPC]
  private void CompleteTask() {
    Task task = GetComponent<Task>();
    task.Complete();
  }
  
  [PunRPC]
  public void AddTask() {
    if (NetworkManager.instance.LocalPlayerPropertyIs("Team", "Robber")) {
      gameObject.GetComponent<MeshRenderer>().material = material;
	}
    base.AddTask();
  }
}
