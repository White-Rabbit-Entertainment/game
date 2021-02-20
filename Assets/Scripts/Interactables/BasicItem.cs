using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BasicItem : PickUpable {

    [PunRPC]
    public void MakeStealable() {
        gameObject.AddComponent<StealableItem>();
        Destroy(this);
    }
}
