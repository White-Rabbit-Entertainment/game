using UnityEngine;
using Photon.Pun;

/// <summary> A reversable interactable has 2 states 0 and 1. It switches
/// between these whilst doing the thing. </summary>
class ReversableInteractable: Interactable {

  private int currentState = 0;

  public override void PrimaryInteraction(Character character) {
    if (currentState == 0) {
      currentState = 1;
    } else {
      currentState = 0;
    }
    base.PrimaryInteraction(character);
  }
}
