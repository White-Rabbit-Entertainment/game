using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Capturable : Interactable {

  public string destination = "/Jail/JailSpawn";

  public override void PrimaryInteraction(GameObject player) {
    Capture();
  }

  public override bool CanInteract() {
    return NetworkManager.instance.LocalPlayerPropertyIs("Team", "Seeker");
  }

  public void Capture() {
    PhotonView view = GetComponent<PhotonView>();
    GameObject jail = GameObject.Find(destination);
    NetworkManager.instance.SetPlayerProperty("Captured", true, view.Owner);
    view.RPC("MovePlayer", view.Owner, jail.transform.position);
  }
}
