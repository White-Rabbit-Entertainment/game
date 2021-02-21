using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Stealable : PickUpable, Taskable {

    public Material stealableMaterial;

    void OnCollisionEnter(Collision collision) {
		  if(collision.gameObject.tag == "endpoint" && PhotonNetwork.LocalPlayer.IsMasterClient) {
        GetComponent<PhotonView>().RPC("Steal", RpcTarget.All);
		  }
	  }
  
    [PunRPC]
    public void AddTask() {
      if (NetworkManager.instance.LocalPlayerPropertyIs("Team", "Robber")) {
			  gameObject.GetComponent<MeshRenderer>().material = stealableMaterial;
		  }
      Task task = gameObject.AddComponent<Task>() as Task;
      Debug.Log("Added task component to gameObject");
      task.description = "Steal the " + gameObject.name;
    }
    
    [PunRPC]
	  void Steal() {
      Task task = GetComponent<Task>();
      if (task != null) {
        task.Complete();
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;
        Destroy(this);
      }
      
	  }
}
