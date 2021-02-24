using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


/// <summary><c>Interactable</c> is the base class for anything which can be
/// interacted with. It add the funcitons for glowing. These can then be called
/// in <c>ItemInteract</c>. </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(PhotonView))]
public abstract class Interactable : MonoBehaviourPun {

  public string taskDescription;
  private Color interactionColour;
  private Color taskColour;
  public float outlineWidth = 5f;
  
  public bool singleUse;
  public Team team = Team.All;
  
  public Animation itemAnimation;

  private Outline outline;

  public bool IsTaskable() {
    return taskDescription != null;
  }

  // Returns true is a task has been applied to this interactable.
  public bool HasTask() {
    Task task = GetComponent<Task>();
    return task != null && !task.isCompleted;
  }

  // The primary action to do when an item is interacted with. At the moment
  // this is when an item is clicked on.
  public virtual void PrimaryInteraction() {
    if (HasTask() && NetworkManager.instance.LocalPlayerPropertyIs<string>("Team", "Robber")) {
      GetComponent<PhotonView>().RPC("CompleteTask", RpcTarget.All);
    }
    
    // Animation
    PlayAnimation();

    // Destory if single use
    if (singleUse) Destroy(this);
  }
  
  // The action to do when an interaction stops. Atm this when the mouse is
  // released.
  public virtual void PrimaryInteractionOff() {}

  /// <summary> Apply glow around item to show it is interactable. </summary>
  public void GlowOn() {
    Debug.Log("Glow on");
    outline.OutlineColor = interactionColour;
    outline.enabled = true;
  }
  
  /// <summary> Remove glow. </summary>
  public void GlowOff() {
    if (HasTask() && NetworkManager.instance.LocalPlayerPropertyIs("Team", "Robber")) {
      outline.OutlineColor = taskColour;
    } else {
      outline.enabled = false;
    }
  }

  // When we remove iteractablility from an item it should stop glowing.
  void OnDestroy() {
    outline.enabled = false;
  }

  /// <summary> Add a task to this item, i.e. Create a tast to
  /// steal this </summary>
  public virtual void AddTask() {
      // Add the Task script to this
      Task task = gameObject.AddComponent<Task>() as Task;

      // All stealing tasks should have the same kind of description
      task.description = taskDescription;

      // Set outline colour and turn on
      if (NetworkManager.instance.LocalPlayerPropertyIs("Team", "Robber")) {
        outline.OutlineColor = taskColour;
        outline.enabled = true;
      }
  }

  [PunRPC]
  public void AddTaskRPC() {
    AddTask();
  }

  public virtual void Start() {
    outline = gameObject.AddComponent<Outline>() as Outline;
    outline.OutlineWidth = outlineWidth;
    outline.enabled = false;

    interactionColour = new Color(1f, 1f, 1f, 1f);
    taskColour = new Color(0f, 1f, 0.3f, 1f);
  }
  
  [PunRPC]
  public void CompleteTask() {
    Task task = GetComponent<Task>();
    task.Complete();
  }

  public virtual void PlayAnimation() {
    if (GetComponent<Animation>() != null) {
      GetComponent<Animation>().Play();
    }
  }
  
  // Return true is the current player can interact with this interatable.
  public virtual bool CanInteract(Character character) {
    if (team == Team.All) return true;
    if (team == character.team) return true;
    if (team == Team.Real && (character is Seeker || character is Robber)) return true;
    return false;
  }

}
