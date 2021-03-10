using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FakeCapturable : Interactable {
  public override void PrimaryInteraction(Character player) {
    Debug.Log("I am an Agent");
  }

  public override bool CanInteract(Character character) {
    return character is Traitor;
  }
}
