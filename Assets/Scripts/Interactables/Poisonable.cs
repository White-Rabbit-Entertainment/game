using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Loyal), typeof(Traitor))]
public class Poisonable : Interactable {
  public override void PrimaryInteraction(Character player) {
    Poison();
  }

  public override bool CanInteract(Character character) {
    return character is Traitor;
  }

  public void Poison() {
    PhotonView view = GetComponent<PhotonView>();
    NetworkManager.instance.SetPlayerProperty("Poisoned", true, view.Owner);
  }

  public bool IsPoisoned() {
    PhotonView view = GetComponent<PhotonView>();
    return NetworkManager.instance.PlayerPropertyIs<bool>("Poisoned", true, view.Owner);
  }
}
