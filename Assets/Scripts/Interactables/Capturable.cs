using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Capturable : Interactable {

  public string destination = "/Jail/JailSpawn";

  public override void PrimaryInteraction(Character player) {
    Capture();
  }

  public override bool CanInteract(Character character) {
    return character is Traitor;
  }

  public void Capture() {
    PhotonView view = GetComponent<PhotonView>();
    GameObject jail = GameObject.Find(destination);
    NetworkManager.instance.SetPlayerProperty("Captured", true, view.Owner);
    view.RPC("MovePlayer", view.Owner, jail.transform.position);
  }
}
