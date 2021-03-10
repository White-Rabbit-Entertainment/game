using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Loyal), typeof(Traitor))]
public class Poisonable : Interactable {

  public override void Reset() {
    singleUse = true;
  }
  
  public override void PrimaryInteraction(Character player) {
    Poison(player);
  }

  public override bool CanInteract(Character character) {
    return character is Traitor && ((Traitor)character).hasPoison;
  }

  public void Poison(Character player) {
    PhotonView view = GetComponent<PhotonView>();
    NetworkManager.instance.SetPlayerProperty("Poisoned", true, view.Owner);
    ((Traitor)player).hasPoison = false;
    Debug.Log("WARNING WARNING SOMEONE HAS BEEN POISONED");
  }

  public bool IsPoisoned() {
    PhotonView view = GetComponent<PhotonView>();
    return NetworkManager.instance.PlayerPropertyIs<bool>("Poisoned", true, view.Owner);
  }
}
