using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


/// <summary><c>Interactable</c> is the base class for anything which can be
/// interacted with. It add the funcitons for glowing. These can then be called
/// in <c>ItemInteract</c>. </summary>
public abstract class Interactable : MonoBehaviourPun {

  // The primary action to do when an item is interacted with. At the moment
  // this is when an item is clicked on.
  public abstract void PrimaryInteraction(GameObject player);
  
  // The action to do when an interaction stops. Atm this when the mouse is
  // released.
  public virtual void PrimaryInteractionOff() {}


  // Return true is the current player can interact with this interatable.
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

  // When we remove iteractablility from an item it should stop glowing.
  void OnDestroy() {
    GlowOff();
  }
}
