using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capturable : Interactable {
  public void Capture() {
    GameManager.instance.OnRobberCapture(gameObject);
  }
}