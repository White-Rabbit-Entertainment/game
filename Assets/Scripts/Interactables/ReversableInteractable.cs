using UnityEngine;
using Photon.Pun;

/// <summary> A reversable interactable has 2 states 0 and 1. It switches
/// between these whilst doing the thing. </summary>
class ReversableInteractable: Interactable {

  private int currentState = 0;
  public string animation0;
  public string animation1;

  public override void PlayItemAnimation() {
    if (currentState == 0) {
      itemAnimation.Play(animation0);
      currentState = 1;
    } else {
      itemAnimation.Play(animation1);
      currentState = 0;
    }
  }
}
