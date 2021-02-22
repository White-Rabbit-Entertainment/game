using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary><c>Stealable</c> extends <c>PickUpable</c> to allow the item to
/// be picked up.</summary>
public class Stealable : PickUpable, Taskable {

    public Material stealableMaterial;
    
    /// <summary> When a stealable item collides with the "endpoint" the item
    /// should be stolen on all clients. </summary>
    void OnCollisionEnter(Collision collision) {
	  if(collision.gameObject.tag == "endpoint" && PhotonNetwork.LocalPlayer.IsMasterClient) {
        // Calls the steal rpc on all clients
        GetComponent<PhotonView>().RPC("Steal", RpcTarget.All);
	  }
	}
  
    /// <summary> Add a task to this item, i.e. Create a tast to steal this
    /// item. </summary>
    [PunRPC]
    public void AddTask() {

      // If you are a robber make it obvious the item is stolen by applying a
      // different material.
      if (NetworkManager.instance.LocalPlayerPropertyIs("Team", "Robber")) {
	    gameObject.GetComponent<MeshRenderer>().material = stealableMaterial;
      }

      // Add the Task script to this
      Task task = gameObject.AddComponent<Task>() as Task;

      // All stealing tasks should have the same kind of description
      task.description = "Steal the " + gameObject.name;
    }
    
    /// <summary> Steals an item, if there is a task associated with stealing
    /// it should be completed here. </summary>
    [PunRPC]
	void Steal() {

      Task task = GetComponent<Task>();

      // If there is a task associated with stealing
      if (task != null) {
        // Then after stealing mark as completed 
        task.Complete();
        
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
