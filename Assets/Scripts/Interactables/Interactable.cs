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
public abstract class Interactable : MonoBehaviourPun {

  public string taskDescription;
  public Color interactionColour;
  public Color taskColour;
  public Color undoTaskColour;

  [Inherits(typeof(Interactable), IncludeBaseType = true, AllowAbstract = true, ExcludeNone = true)]
  public List<TypeReference> softRequirementTypes;

  public bool canBeMasterTask = true;
  public List<Interactable> hardRequirements;
  
  public bool singleUse;
 
  public Team team = Team.All;
  public Team taskTeam = Team.All;

  public float outlineWidth = 5f;
  
  public bool inRange = false;
  
  public string itemAnimationTrigger;
  public string playerAnimationTrigger;

  private Outline outline;
  private Target taskMarker;
  private Target undoneMarker;

  // Target values
  [SerializeField] private Sprite taskMarkerBoxImageOverride;
  [SerializeField] private Sprite taskMarkerArrowImageOverride;
  [SerializeField] private string taskMarkerBoxTextOverride = "TASK";
  [SerializeField] private string taskMarkerArrowTextOverride;
  [SerializeField] private Color taskMarkerColour = Color.green;

  [SerializeField] private Sprite undoneMarkerBoxImageOverride = Resources.Load<Sprite>("/Assets/Resources/undoneMarker");
  [SerializeField] private Sprite undoneMarkerArrowImageOverride;
  [SerializeField] private string undoneMarkerBoxTextOverride = "UNDONE";
  [SerializeField] private string undoneMarkerArrowTextOverride;
  [SerializeField] private Color undoneMarkerColour = Color.green;
  
  public Task task;
  public PhotonView View {
    get { return GetComponent<PhotonView>(); }
  }

  public virtual void Reset() {}

  public virtual void Start() {
    outline = gameObject.AddComponent<Outline>() as Outline;
    outline.OutlineWidth = outlineWidth;
    outline.enabled = false;
  
    taskMarker = gameObject.AddComponent<Target>() as Target;
    taskMarker.enabled = false;
    taskMarker.boxImage = taskMarkerBoxImageOverride;
    taskMarker.boxText = taskMarkerBoxTextOverride;
    taskMarker.arrowImage = taskMarkerArrowImageOverride;
    taskMarker.arrowText = taskMarkerArrowTextOverride;
    taskMarker.TargetColor = taskMarkerColour;

    undoneMarker = gameObject.AddComponent<Target>() as Target;
    undoneMarker.enabled = false;
    undoneMarker.boxImage = undoneMarkerBoxImageOverride;
    undoneMarker.boxText = undoneMarkerBoxTextOverride;
    undoneMarker.arrowImage = undoneMarkerArrowImageOverride;
    undoneMarker.arrowText = undoneMarkerArrowTextOverride;
    undoneMarker.TargetColor = undoneMarkerColour;

    interactionColour = new Color(1f, 1f, 1f, 1f);
    taskColour = new Color(0f, 1f, 0.3f, 1f);
    undoTaskColour = new Color(1f, 0f, 0f, 1f);
  }

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

  [PunRPC]
  public void SetTaskGlowRPC() {
    SetTaskGlow();
  }

  public virtual void SetTaskGlow() {
    //if (inRange) {
    //  Team team = NetworkManager.instance.GetLocalPlayerProperty<Team>("Team");
    //  if (team == Team.Traitor && HasUndoTask()) {
    //    SetGlow(undoTaskColour);
    //  } else if (HasTask() && task.AllChildrenCompleted()) {
    //    SetGlow(taskColour);
    //  } else {
    //    outline.enabled = false;
    //  }
    //} else {
    //  outline.enabled = false;
    //}
    outline.enabled = false;
  }

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
    SetTaskGlow();
  }

  public void EnableTaskMarker() {
    taskMarker.enabled = true;
  }
  
  public void DisableTaskMarker() {
    taskMarker.enabled = false;
  }

  public void EnableUndoneMarker() {
    undoneMarker.enabled = true;
  }
  
  public void DisableUndoneMarker() {
    undoneMarker.enabled = false;
  }

  // Once completed set the disabled state
  public virtual void OnParentTaskComplete(Character character = null) {}
  
  // When the task is readded set the enabled state
  public virtual void OnParentTaskUncomplete() {}

  // When we remove iteractablility from an item it should stop glowing.
  void OnDestroy() {
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
      
      // Set outline colour and turn on
      View.RPC("SetTaskGlowRPC", RpcTarget.All);
      if (parentTask != null) {
        task.parent = parentTask;
      } else {
        task.taskManager.AddTask(task);
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
  
  [PunRPC]
  public void AddCompletedTaskRPC() {
    AddTaskRPC();
    if (PhotonNetwork.IsMasterClient) {
      task.Complete(true);
    }
  }
  
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
  
  //  public void CompleteTask() {
  //   task.Complete();
  // }

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
      View.RPC("ItemAnimationTrigger", RpcTarget.All);
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
    if (character is Loyal && ((Loyal)character).assignedSubTask == task) return true;
    if (character is Traitor && (HasUndoTask() || (HasTask() && task.IsMasterTask() && task.AllChildrenCompleted()))) return true;
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
      foreach(TypeReference type in softRequirementTypes) {
        if (interactable.GetComponent(type.Type) != null) hasCorrectType = true;
      }
      if (interactable.GetComponent<Interactable>() != null
      && hasCorrectType
      && !interactable.GetComponent<Interactable>().HasTask()) {
        softRequirements.Add(interactable.GetComponent<Interactable>());
      }
    }
    return softRequirements;
  }

}
