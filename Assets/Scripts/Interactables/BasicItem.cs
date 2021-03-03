using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BasicItem : Pickupable {

    [PunRPC]
    public void MakeStealable() {
        gameObject.AddComponent<Stealable>();
        Destroy(this);
    }
}
