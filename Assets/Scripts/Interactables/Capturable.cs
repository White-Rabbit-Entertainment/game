using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Capturable : Interactable {
  public void Capture() {
    //Debug.Log(GetComponent<PhotonView>());
    GameManager.instance.OnRobberCapture(gameObject);
  }
}