using UnityEngine;
using Photon.Pun;

/// <summary> A reversable interactable has 2 states 0 and 1. It switches
/// between these whilst doing the thing. </summary>
class ReversableInteractable: Interactable {

  private int currentState = 0;
  public string itemAnimationBool;

  public override void PrimaryInteraction(Character character) {
    if (currentState == 0) {
      currentState = 1;
    } else {
      currentState = 0;
    }
    base.PrimaryInteraction(character);
  }

  public override void PlayItemAnimation() {
    Animator animator = GetComponentInChildren<Animator>();
    view.TransferOwnership(PhotonNetwork.LocalPlayer);
    if (itemAnimationTrigger != null && itemAnimationTrigger != "" && animator != null) {
      if (currentState == 0) {
        animator.SetBool(itemAnimationBool, true);
      } else {
        animator.SetBool(itemAnimationBool, false);
      }
    }
  }
}
