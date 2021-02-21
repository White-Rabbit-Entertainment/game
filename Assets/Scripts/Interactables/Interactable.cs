using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public abstract class Interactable : MonoBehaviourPun {
  void Start() {
      gameObject.tag = "interactable";
  }

  public void GlowOn() {
    GetComponent<Outline>().enabled = true;
  }

  public void GlowOff() {
    GetComponent<Outline>().enabled = false;
  }
}
