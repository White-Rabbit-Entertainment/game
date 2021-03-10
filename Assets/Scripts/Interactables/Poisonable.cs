using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Poisonable : Interactable {

  public Meal meal;

  public override void Start() {
    base.Start();
    meal = gameObject.AddComponent<Meal>() as Meal;
  }

  public override void Reset() {
    singleUse = true;
  }

  public override void PrimaryInteraction(Character player) {
    view.RPC("Poison", RpcTarget.All);
  }

  public override bool CanInteract(Character character) {
    return character is Traitor && ((Traitor)character).hasPoison;
  }

  public bool IsPoisoned() {
    PhotonView view = GetComponent<PhotonView>();
    return NetworkManager.instance.PlayerPropertyIs<bool>("Poisoned", true, view.Owner);
  }
}
