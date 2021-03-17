using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PlayableCharacter))]
public class Poisonable : Interactable {
  PlayableCharacter player;

  public void Start() {
    base.Start();
    player = GetComponent<PlayableCharacter>();
  }

  public override void Reset() {
    singleUse = true;
  }

  public override void PrimaryInteraction(Character poisoningPlayer) {
    PhotonView mealView = player.GetMeal().GetComponent<PhotonView>();
    mealView.RPC("Poison", RpcTarget.All);
    ((Traitor)poisoningPlayer).hasPoison = false;
    view.RPC("Kill", ((PlayableCharacter)poisoningPlayer).owner);
  }

  public override bool CanInteract(Character character) {
    return character is Traitor && ((Traitor)character).hasPoison;
  }

  public bool IsPoisoned() {
    return player.GetMeal().isPoisoned;
  }
}
