using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Floodable : Sabotageable
{
    public GameObject water;

    [PunRPC]
    public override void Sabotage() {
        water.SetActive(true);
        base.Sabotage();
    }

    [PunRPC]
    public override void Fix() { 
        water.SetActive(false); 
        base.Fix(); 
    }
}

