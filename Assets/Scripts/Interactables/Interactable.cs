using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TypeReferences;
using System;
using Photon.Pun;


/// <summary><c>Interactable</c> is the base class for anything which can be
/// interacted with. It add the funcitons for glowing and targets.
/// The interface between Characters and Interactables is <c>ItemInteract</c>. 
///
/// The key methods are:
///  - PrimaryInteraction (and PrimaryInteractionOff) - Handles the actaul
///    interaction between a character and the item
///  - CanInteract - Dictates whether a character can interat with the item
///
/// </summary>
[DisallowMultipleComponent]
public abstract class Interactable : MonoBehaviourPun {

  // The description of the task, shown in TaskUI
  public string taskDescription;

  // Colours
  public Color interactionColour;
  public Color taskColour;
  public Color undoTaskColour;

  // When a task is assigned to this item its hard requirements also get given
  // tasks and they become requirements of this taks.
  // A soft requirement can also be added to the list of requirements.
  public List<Interactable> hardRequirements;
  
  // A list of types which an added hard requirement can be 
  [Inherits(typeof(Interactable), IncludeBaseType = true, AllowAbstract = true, ExcludeNone = true)]
  public List<TypeReference> softRequirementTypes;
  // The probability a soft requirement is added to the hard requirements list
  public float softRequirementProbability = 0.5f;

  // Whether the task can be a task without a parent
  public bool canBeMasterTask = true;
 
  // If the interactable should destory it iteractable script after an
  // interaction. I.e it can only be interacted with once
  public bool singleUse;
  public bool interactionDisabled = false;

  // Which teams can interact with the item
  public Team team = Team.All;
  public Team taskTeam = Team.All;

  // The width of the outline on the interactable
  public float outlineWidth = 5f;
  // Allow setting of override (instead of adding at start), this is handy for
  // adding outline to different part of gameobject or precomputed outlines.
  public bool manualOutline = false;
  [SerializeField] public Outline outline = null;
 
  // Is this item currently in range of the local player
  public bool inRange = false;
 
  // Animations
  public string itemAnimationTrigger;
  public string playerAnimationTrigger;

  // Targets, these are used to guide the player to the item
  protected Target taskMarker;
  protected Target undoneMarker;
 
  // The task assigned to the interactable
  public Task task;

  public PhotonView View {
    get { return GetComponent<PhotonView>(); }
  }

  public virtual void Reset() {}

  public virtual void Start() {
    // Set outline, if override provided use that, if not create an outline.
    // Overrides are used to set the outline to a precomputed outline
    if (outline == null && !manualOutline) {
      outline = gameObject.AddComponent<Outline>() as Outline;
    }
    if (outline) {
      outline.OutlineWidth = outlineWidth;
      outline.enabled = false;
    }
 
    // Setup markers
    taskMarker = gameObject.AddComponent<Target>() as Target;
    taskMarker.enabled = false;
    taskMarker.boxText = "TASK";
    taskMarker.TargetColor = Color.green;

    undoneMarker = gameObject.AddComponent<Target>() as Target;
    undoneMarker.enabled = false;
    undoneMarker.NeedDistanceText = false;
    undoneMarker.NeedArrowIndicator = false;
    undoneMarker.boxImage = Resources.Load<Sprite>("Images/exclaimationmark");
    undoneMarker.boxText = "UNDONE";
    undoneMarker.TargetColor = Color.red;

    // Colours
    interactionColour = new Color(1f, 1f, 1f, 1f);
    taskColour = new Color(0f, 1f, 0.3f, 1f);
    undoTaskColour = new Color(1f, 0f, 0f, 1f);
  }

  // Can this item be assigned a task
  public bool IsTaskable() {
    return taskDescription != null;
  }

  // Returns true is a task has been applied to this interactable.
  public bool HasTask() {
    return task != null && !task.isCompleted;
  }
  
  // Returns true if the task has been completed and can be undone
  public bool HasUndoTask() {
    return task != null && task.isCompleted && task.isUndoable;
  }

  // The primary action to do when an item is interacted with. At the moment
  // this is when an item is clicked on.
  public virtual void PrimaryInteraction(Character character) {
    // If this task is done and can be undone then set the traitors primary
    // interaction to traitor undo.
    if (HasUndoTask() && character is Traitor) {
      TraitorUndo(character);
    } else if (HasTask() && !(character is Agent)) {
      task.Complete();
    }
      
    // Animation
    PlayItemAnimation();
    PlayCharacterAnimation(character);

    // Destory if single use
    if (singleUse) View.RPC("Disable", RpcTarget.All);
  }

  // The action to do when an interaction stops. Atm this when the mouse is
  // released.
  public virtual void PrimaryInteractionOff(Character character) {}
  
  public virtual void TraitorUndo(Character character) {
    task.Uncomplete();
  }

  // Turn on the glow for this item and set it to the provided colour
  public void SetGlow(Color colour) {
    outline.OutlineColor = colour;
    outline.enabled = true;
  }

  /// <summary> Apply glow around item to show it is interactable. </summary>
  public void InteractionGlowOn() {
    SetGlow(interactionColour);
  }
  
  /// <summary> Remove glow. </summary>
  public void InteractionGlowOff() {
    outline.enabled = false;
  }

  public void EnableTaskMarker() {
    DisableUndoneMarker();
    taskMarker.enabled = true;
  }
  
  public void DisableTaskMarker() {
    taskMarker.enabled = false;
  }

  public void EnableUndoneMarker() {
    if (!(NetworkManager.instance.GetMe() is Traitor)) undoneMarker.enabled = true;
  }
  
  public void DisableUndoneMarker() {
    undoneMarker.enabled = false;
  }

  // Once completed set the disabled state
  public virtual void OnParentTaskComplete(Character character = null) {}
  
  // When the task is readded set the enabled state
  public virtual void OnParentTaskUncomplete() {}

  // When we remove iteractablility from an item it should stop glowing.
  protected virtual void OnDestroy() {
    outline.enabled = false;
  }

  /// <summary> Add a task to this item, i.e. Create a tast to
  /// steal this </summary>
  public virtual void AddTask(Task parentTask = null) {
      
      // If this already has a task then just use that 
      if (this.task != null) {
        Debug.Log($"You are trying to add another task to {gameObject}");
        return;
      }

      task = gameObject.AddComponent<Task>() as Task;

      // All stealing tasks should have the same kind of description
      task.description = taskDescription;

      if (hardRequirements != null && hardRequirements.Count > 0) {
        foreach(Interactable requirement in hardRequirements) {

          // Assign a new task to the requirement
          requirement.AddTask(task);

          // Add this as a requirement to the task. I.e. before this task can
          // be completed this requirement must be done first.
          task.requirements.Add(requirement.task);
        }
      }
      
      if (parentTask != null) {
        task.parent = parentTask;
      } else {
        if (!(this is Sabotageable)) {
          task.taskManager.AddTask(task);
        }
      }
  }

  // for master tasks (tasks with no parents).
  [PunRPC]
  public void AddTaskRPC() {
    AddTask();
    if (!task.IsMasterTask()) {
      throw new Exception("AddTaskRPC cannot be used on a subtask.");
    }
  }
 
  // Add a task and complete it
  [PunRPC]
  public void AddCompletedTaskRPC() {
    AddTaskRPC();
    if (PhotonNetwork.IsMasterClient) {
      task.Complete(true);
    }
  }
 
  // Add a task with a timelimit
  [PunRPC]
  public void AddTaskWithTimerRPC(Timer timer) {
    AddTaskRPC();
    task.timer = timer;
  }

  [PunRPC]
  public void Disable() {
    taskTeam = Team.None;
    team = Team.None;
  }

  [PunRPC]
  public void PlayerAnimationTrigger() {}

  [PunRPC]
  public void ItemAnimationTrigger() {
    Animator animator = GetComponent<Animator>();
    animator.SetTrigger(itemAnimationTrigger);
  }

  // Start the animation on the interactable as a result of an interaction (eg turning wheel of ship)
  public virtual void PlayItemAnimation() {
    Animator animator = GetComponent<Animator>();
    if (itemAnimationTrigger != null && itemAnimationTrigger != "" && animator != null) {
      View.RPC("ItemAnimationTrigger", RpcTarget.All);
    }
  }
  
  // Start the animation on a player as a result of an interaction (eg pickup animation)
  public virtual void PlayCharacterAnimation(Character character) {
    if (playerAnimationTrigger != null && playerAnimationTrigger != "") {
      Animator animator = character.GetComponentInChildren<Animator>();
      animator.SetTrigger(playerAnimationTrigger);
    }
  }
  
  // Return true is the current player can interact with this interatable.
  public virtual bool CanInteract(Character character) {
    if (interactionDisabled) return false;
    if (character is Loyal && ((Loyal)character).assignedSubTask == task) return true;
    if (character is Traitor && (HasUndoTask() || task == null)) return true;
    if (character is Agent && task == null) return true;
    if ((character is Loyal || character is Traitor) && this is Votable && GetComponent<PlayableCharacter>() != character) return true;
    return false;
  }
  
  //// Taks Requirements
  [PunRPC]
  public void AddHardRequirement(int viewId) {
    PhotonView itemView = PhotonView.Find(viewId);
    hardRequirements.Add(itemView.gameObject.GetComponent<Interactable>());
  }

  // Picks one of the soft requirements for this task to be the hard
  // requirement
  public void PickHardRequirements(List<Transform> interactables) {
    List<Interactable> softRequirements = GetSoftRequirements(interactables);
    if (softRequirements.Count > 0) {
      System.Random random = new System.Random(System.Guid.NewGuid().GetHashCode());
      int randomIndex = random.Next(softRequirements.Count);
      View.RPC("AddHardRequirement", RpcTarget.All, softRequirements[randomIndex].GetComponent<PhotonView>().ViewID);
    }
  }

  // Returns true is this task can have any soft requirements
  public virtual bool HasSoftRequirements() {
    return softRequirementTypes != null && softRequirementTypes.Count != 0;
  }

  // Given a list of interactables returns a list of all interactbles which are
  // allowed to be requirements for this task.
  public virtual List<Interactable> GetSoftRequirements(List<Transform> interactables) {
    List<Interactable> softRequirements = new List<Interactable>();
    foreach (Transform interactable in interactables) {
      bool hasCorrectType = false;
      // Check if item is valid type
      foreach(TypeReference type in softRequirementTypes) {
        if (interactable.GetComponent(type.Type) != null) hasCorrectType = true;
      }

      // Check item can be a soft requirement
      if (interactable.GetComponent<Interactable>() != null
      && hasCorrectType
      && interactable.GetComponent<Interactable>().task == null) {
        softRequirements.Add(interactable.GetComponent<Interactable>());
      }
    }
    return softRequirements;
  }

  // Handle an item coming close to a player
  public virtual void OnEnterPlayerRadius() {
    if (task != null && task.isUndone && NetworkManager.instance.GetMe().assignedSubTask != task) {
      task.EnableUndoneMarker();
    }
    inRange = true;
  }
  
  // Handle an going far away from a player
  public virtual void OnExitPlayerRadius() {
    if (task != null) task.DisableUndoneMarker();
    inRange = false;
  }

}
