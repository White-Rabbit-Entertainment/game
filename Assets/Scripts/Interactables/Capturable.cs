using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Capturable : Interactable {
  public override void PrimaryInteraction() {
    Capture();
  }

  public override bool CanInteract() {
    return NetworkManager.instance.LocalPlayerPropertyIs("Team", "Seeker");
  }

  public void Capture() {
    GameManager.instance.OnRobberCapture(gameObject);
  }
}
