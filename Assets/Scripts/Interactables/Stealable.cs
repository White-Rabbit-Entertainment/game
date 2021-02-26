using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary><c>Stealable</c> extends <c>PickUpable</c> to allow the item to
/// be picked up.</summary>
public class Stealable : PickUpable {

    public override void Reset() {
      taskDescription = "Steal the " + this.name;
      base.Reset();
    }

    /// <summary> When a stealable item collides with the "endpoint" the item
    /// should be stolen on all clients. </summary>
    void OnCollisionEnter(Collision collision) {
	  if(collision.gameObject.tag == "endpoint" && PhotonNetwork.LocalPlayer.IsMasterClient) {
        // Calls the steal rpc on all clients
        GetComponent<PhotonView>().RPC("Steal", RpcTarget.All);
	  }
	}
    
    /// <summary> Steals an item, if there is a task associated with stealing
    /// it should be completed here. </summary>
    [PunRPC]
	void Steal() {

      Task task = GetComponent<Task>();

      // If there is a task associated with stealing
      if (task != null) {
        // Then after stealing mark as completed 
        CompleteTask();
        
        // Then make it so the item cant be moved 
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;

        // After at item is stolen we dont want to make it interactable any
        // more so we can destory this script, which removes all
        // interactability.
        Destroy(this);
      }
	}
}
