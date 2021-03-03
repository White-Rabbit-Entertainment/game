using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TypeReferences;
using System;
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

  [Inherits(typeof(Interactable), IncludeBaseType = true, AllowAbstract = true, ExcludeNone = true)]
  public List<TypeReference> softRequirementTypes;

  public bool canBeMasterTask = true;
  public float outlineWidth = 5f;
  public Interactable hardRequirement;
  
  public bool singleUse;
  public Team team = Team.All;
  
  public string itemAnimationTrigger;
  public string playerAnimationTrigger;

  protected Outline outline;
  protected PhotonView view;
  protected Task task;

  [PunRPC]
  public void AssignHardRequirement(int viewId) {
    PhotonView itemView = PhotonView.Find(viewId);
    hardRequirement = itemView.gameObject.GetComponent<Interactable>();
  }

  public void PickHardRequirement(List<Transform> interactables) {
    List<Interactable> softRequirements = GetSoftRequirements(interactables);
    if (hardRequirement == null && softRequirements.Count > 0) {
      System.Random random = new System.Random(System.Guid.NewGuid().GetHashCode());
      int randomIndex = random.Next(softRequirements.Count);
      view.RPC("AssignHardRequirement", RpcTarget.All, softRequirements[randomIndex].GetComponent<PhotonView>().ViewID);
    }
  }

  public virtual bool HasSoftRequirements() {
    return softRequirementTypes != null && softRequirementTypes.Count != 0;
  }

  public virtual List<Interactable> GetSoftRequirements(List<Transform> interactables) {
    List<Interactable> softRequirements = new List<Interactable>();
    Debug.Log("started softies");
    foreach (Transform interactable in interactables) {
      bool hasCorrectType = false;
      foreach(TypeReference type in softRequirementTypes) {
        System.Type realType = type.Type;
        var getComponentMethod = realType.GetMethod("GetComponent", new []{typeof(GameObject)});
        var getComponentReference = getComponentMethod.MakeGenericMethod(new []{typeof(GameObject)});
        var component = getComponentReference.Invoke(interactable, new object[]{new GameObject()});
        if (component != null) hasCorrectType = true;
        //if (interactable.GetComponent<realType>() == null) hasCorrectType = true;
      }
      Debug.Log("started for loop");
      Debug.Log("1: " + (interactable.GetComponent<Interactable>() != null));
      Debug.Log("2: " + hasCorrectType);
      Debug.Log("3: " + !interactable.GetComponent<Interactable>().HasTask());
      if (interactable.GetComponent<Interactable>() != null
      && hasCorrectType
      && !interactable.GetComponent<Interactable>().HasTask()) {
        Debug.Log("in if statement");
        softRequirements.Add(interactable.GetComponent<Interactable>());
      }
    }
    Debug.Log("finished softies");
    return softRequirements;
  }

  public bool IsTaskable() {
    return taskDescription != null;
  }

  // Returns true is a task has been applied to this interactable.
  public bool HasTask() {
    return task != null && !task.isCompleted;
  }

  // The primary action to do when an item is interacted with. At the moment
  // this is when an item is clicked on.
  public virtual void PrimaryInteraction(Character character) {
    if (HasTask() && NetworkManager.instance.LocalPlayerPropertyIs<string>("Team", "Robber")) {
      view.RPC("CompleteTask", RpcTarget.All);
    }
    
    // Animation
    PlayItemAnimation();
    PlayCharacterAnimation(character);

    // Destory if single use
    if (singleUse) Destroy(this);
  }
  
  // The action to do when an interaction stops. Atm this when the mouse is
  // released.
  public virtual void PrimaryInteractionOff(Character character) {}

  /// <summary> Apply glow around item to show it is interactable. </summary>
  public void GlowOn() {
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
      task = gameObject.AddComponent<Task>() as Task;

      // All stealing tasks should have the same kind of description
      task.description = taskDescription;

      // Set outline colour and turn on
      if (NetworkManager.instance.LocalPlayerPropertyIs("Team", "Robber")) {
        outline.OutlineColor = taskColour;
        outline.enabled = true;
      }

      if (hardRequirement != null) {
        hardRequirement.AddTask();
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
    view = GetComponent<PhotonView>();

    interactionColour = new Color(1f, 1f, 1f, 1f);
    taskColour = new Color(0f, 1f, 0.3f, 1f);
  }
  
  [PunRPC]
  public void CompleteTask() {
    task.Complete();
  }

  [PunRPC]
  public void PlayerAnimationTrigger() {

  }

  [PunRPC]
  public void ItemAnimationTrigger() {
    Animator animator = GetComponent<Animator>();
    animator.SetTrigger(itemAnimationTrigger);
  }

  public virtual void PlayItemAnimation() {
    Animator animator = GetComponent<Animator>();
    if (itemAnimationTrigger != null && itemAnimationTrigger != "" && animator != null) {
      view.RPC("ItemAnimationTrigger", RpcTarget.All);
    }
  }
  
  public virtual void PlayCharacterAnimation(Character character) {
    if (playerAnimationTrigger != null && playerAnimationTrigger != "") {
      Animator animator = character.GetComponentInChildren<Animator>();
      animator.SetTrigger(playerAnimationTrigger);
    }
  }
  
  // Return true is the current player can interact with this interatable.
  public virtual bool CanInteract(Character character) {
    if (hardRequirement != null && HasTask() && !hardRequirement.task.isCompleted) return false;
    return team.HasFlag(character.team);
  }
}
