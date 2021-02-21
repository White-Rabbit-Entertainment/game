using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BasicItem : PickUpable {

    public Material stealableMaterial;

    [PunRPC]
    public void MakeStealable() {
        gameObject.AddComponent<Stealable>();
        if (NetworkManager.instance.LocalPlayerPropertyIs("Team", "Robber")) {
			gameObject.GetComponent<MeshRenderer>().material = stealableMaterial;
		}
        Destroy(this);
    }
}
