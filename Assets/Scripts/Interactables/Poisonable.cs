using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Poisonable : Interactable {
  public Meal meal;

  public override void Reset() {
    singleUse = true;
  }

  public override void PrimaryInteraction(Character player) {
    PhotonView mealView = meal.GetComponent<PhotonView>();
    mealView.RPC("Poison", RpcTarget.All);
    ((Traitor)player).hasPoison = false;
  }

  public override bool CanInteract(Character character) {
    return character is Traitor && ((Traitor)character).hasPoison;
  }

  public bool IsPoisoned() {
    return meal.isPoisoned;
  }
}
