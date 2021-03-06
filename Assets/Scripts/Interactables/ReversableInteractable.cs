using UnityEngine;
using Photon.Pun;

/// <summary> A reversable interactable has 2 states 0 and 1. It switches
/// between these whilst doing the thing. </summary>
class ReversableInteractable: Interactable {
  public string itemAnimationBool;

  public override void PlayItemAnimation() {
    view.TransferOwnership(PhotonNetwork.LocalPlayer);
    Animator animator = GetComponentInChildren<Animator>();
    if (itemAnimationBool != null && itemAnimationBool != "" && animator != null) {
      if (animator.GetBool(itemAnimationBool)) {
        animator.SetBool(itemAnimationBool, false);
      } else {
        animator.SetBool(itemAnimationBool, true);
      }
    }
  }
}
