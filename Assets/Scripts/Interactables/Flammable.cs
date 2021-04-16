using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Flammable : Sabotageable
{
    public GameObject smallFires;

    [PunRPC]
    public override void Sabotage() {
        base.Sabotage();
        smallFires.SetActive(true);
    }

    [PunRPC]
    public override void Fix(int fixPlayerViewId) {   
        base.Fix(fixPlayerViewId);
        if (!isSabotaged) smallFires.SetActive(false);
    }
}
