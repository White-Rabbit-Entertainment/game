using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class Interactable : MonoBehaviour {
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
