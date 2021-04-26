using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Floodable : Sabotageable
{
    public GameObject water;

    [PunRPC]
    public override void Sabotage() {
        StartCoroutine(StartFlood());
        base.Sabotage();
    }

    [PunRPC]
    public override void Fix() { 
        water.SetActive(false); 
        base.Fix(); 
    }

    public IEnumerator StartFlood(){
        yield return new WaitForSeconds(5);
        water.SetActive(true);
    }
}

