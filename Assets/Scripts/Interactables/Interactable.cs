using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


/// <summary><c>Interactable</c> is the base class for anything which can be
/// interacted with. It add the funcitons for glowing. These can then be called
/// in <c>ItemInteract</c>. </summary>
[DisallowMultipleComponent]
public abstract class Interactable : MonoBehaviourPun {

  public string taskDescription;
  public Color interactionColour = new Color(255,255,255,0);
  public Color taskColour = new Color(255, 255, 255, 0);

  public bool IsTaskable() {
    return taskDescription != null;
  }

  // Returns true is a task has been applied to this interactable.
  public bool HasTask() {
    return GetComponent<Task>() != null;
  }

  // The primary action to do when an item is interacted with. At the moment
  // this is when an item is clicked on.
  public abstract void PrimaryInteraction();
  
  // The action to do when an interaction stops. Atm this when the mouse is
  // released.
  public virtual void PrimaryInteractionOff() {}

  // Return true is the current player can interact with this interatable.
  public virtual bool CanInteract() {
    return true;
  }
  
  /// <summary> Apply glow around item to show it is interactable. </summary>
  public void GlowOn() {
    GetComponent<Outline>().OutlineColor = interactionColour;
    GetComponent<Outline>().enabled = true;
  }
  
  /// <summary> Remove glow. </summary>
  public void GlowOff() {
    if (HasTask()) {
      GetComponent<Outline>().OutlineColor = taskColour;
    } else {
      GetComponent<Outline>().enabled = false;
    }
  }

  // When we remove iteractablility from an item it should stop glowing.
  void OnDestroy() {
    GetComponent<Outline>().enabled = false;
  }

  /// <summary> Add a task to this item, i.e. Create a tast to
  /// steal this </summary>
  [PunRPC]
  public virtual void AddTask() {
      // Add the Task script to this
      Task task = gameObject.AddComponent<Task>() as Task;

      // All stealing tasks should have the same kind of description
      task.description = taskDescription;
      

  }
}
