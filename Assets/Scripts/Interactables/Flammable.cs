using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Flammable : Sabotageable
{
    public GameObject smallFires;

    [PunRPC]
    public override void Sabotage() {
        StartCoroutine(StartFire());
        base.Sabotage();
         
    }

    [PunRPC]
    public override void Fix() {  
        smallFires.SetActive(false);
        base.Fix(); 
    }

    public IEnumerator StartFire(){
        yield return new WaitForSeconds(sabotageDelaySeconds);
        smallFires.SetActive(true); 
    }



}
