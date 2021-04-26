using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Floodable : Sabotageable
{
    public GameObject water;

    [PunRPC]
    public override void Sabotage() {
        base.Sabotage();
        water.SetActive(true);
    }

    [PunRPC]
    public override void Fix(int fixPlayerViewId) {   
        base.Fix(fixPlayerViewId);
        if (!isSabotaged) water.SetActive(false);
    }
}

