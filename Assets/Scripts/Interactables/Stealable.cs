using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class Stealable : PickUpable {
    void OnCollisionEnter(Collision collision) {
		if(collision.gameObject.tag == "endpoint" && PhotonNetwork.LocalPlayer.IsMasterClient)
			GameManager.instance.OnItemInSafeZone(gameObject);
	}
}