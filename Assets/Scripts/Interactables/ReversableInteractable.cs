using UnityEngine;
using Photon.Pun;

/// <summary> A reversable interactable has 2 states 0 and 1. It switches
/// between these whilst doing the thing. </summary>
class ReversableInteractable: Interactable {

  private int currentState = 0;
  public string itemAnimationBool = "Open";

  public override void PrimaryInteraction(Character character) {
    if (currentState == 0) {
      currentState = 1;
    } else {
      currentState = 0;
    }
    base.PrimaryInteraction(character);
  }

  public virtual void PlayItemAnimation() {
    Animator animator = GetComponentInChildren<Animator>();
    if (itemAnimationTrigger != null && itemAnimationTrigger != "" && animator != null) {
      if (currentState == 0) {
        animator.SetBool(itemAnimationBool, true);
      } else {
        animator.SetBool(itemAnimationBool, false);
      }
    }
  }
}
