using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


/// <summary><c>Interactable</c> is the base class for anything which can be
/// interacted with. It add the funcitons for glowing. These can then be called
/// in <c>ItemInteract</c>. </summary>
public abstract class Interactable : MonoBehaviourPun {

  public abstract void PrimaryInteraction();
  public virtual void PrimaryInteractionOff() {}
  public virtual bool CanInteract() {
    return true;
  }
  
  /// <summary> Apply glow around item to show it is interactable. </summary>
  public void GlowOn() {
    GetComponent<Outline>().enabled = true;
  }

  /// <summary> Remove glow. </summary>
  public void GlowOff() {
    GetComponent<Outline>().enabled = false;
  }
}
