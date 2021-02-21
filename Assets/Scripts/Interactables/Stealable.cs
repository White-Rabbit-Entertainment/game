using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class Stealable : PickUpable, Taskable {
 
    private bool isStolen = false;
    public bool isCompleted {get {return isStolen;}}

    void OnCollisionEnter(Collision collision) {
		if(collision.gameObject.tag == "endpoint" && PhotonNetwork.LocalPlayer.IsMasterClient) {
	      GameManager.instance.OnItemInSafeZone(gameObject);
		}
	}
    
    [PunRPC]
    public void Complete() {
      isStolen = true;
    }
    
    [PunRPC]
    public void AddTask(string description = null) {
      Task task = gameObject.AddComponent<Task>() as Task;

      if (description == null) {
        description = "Steal the " + gameObject.name;
      }
      task.description = description;
    }
    
    [PunRPC]
	  void Steal() {
      Complete();
      Rigidbody rb = gameObject.GetComponent<Rigidbody>();
      rb.constraints = RigidbodyConstraints.FreezeAll;
      Destroy(this);
	  }
}
