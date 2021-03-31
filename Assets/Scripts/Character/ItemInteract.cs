using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

 
/// <summary><c>ItemInteract</c> is the class which defines the behaviour for
/// how a player interacts with an <c>Interactable</c>. 
/// E.g. Defines when to turn on glow and when to pickup a PickUpable
[RequireComponent(typeof(PlayableCharacter))]
public class ItemInteract : MonoBehaviourPun {

    public float maxInteractionDistance = 2f;
    [SerializeField] private Transform cameraTransform;

    private RaycastHit raycastFocus;
    private bool interactableInRange = false;
    private Interactable currentInteractable;
    private PlayableCharacter character;

    public SphereCollider itemCollider;

    void Start() {
        if (!photonView.IsMine) {
            Destroy(GetComponent<AudioListener>());
            Destroy(this);
        } else {
            character = GetComponent<PlayableCharacter>();
            itemCollider.enabled = true;
        }
    }

    public List<Interactable> interactablesInRange = new List<Interactable>();
    public List<Interactable> possibleInteractables = new List<Interactable>();
    
    public Interactable GetBestInteractable() {
        float angle = float.PositiveInfinity;
        Interactable bestInteractable = null;
        foreach(Interactable interactable in possibleInteractables) {
            RaycastHit hit;
            Physics.Linecast(cameraTransform.position, interactable.transform.position, out hit); 
            float interactableAngle = Vector3.Angle(cameraTransform.forward, interactable.transform.position - cameraTransform.position);
            //Debug.Log(hit.collider.GetComponent<Interactable>());
            if (interactableAngle < angle && hit.collider != null && hit.collider.GetComponent<Interactable>() == interactable) {
                angle = interactableAngle;
                bestInteractable = interactable;
            }
        }
        return bestInteractable;

    }

 
    void Update() {
        Interactable newInteractable = null; 
        // We can only interact with an item if the item is in reach and we are
        // not currently holding an item.
        if (!character.HasItem()) {
            newInteractable = GetBestInteractable();
        }

        // If we are able to interact with stuff
        if (newInteractable != null) {
            // Interactable newInteractable = raycastFocus.collider.transform.GetComponent<Interactable>();
            // If we are already interacting with something but we are now
            // trying to interact with something new, then we need to disable
            // the other interaction (turn off its glow).
            if (newInteractable != currentInteractable && currentInteractable != null) {
                currentInteractable.InteractionGlowOff();
            }
            currentInteractable = newInteractable;
            
            if (currentInteractable != null && currentInteractable.CanInteract(character)) {
                // If we are able to interact with the new interactable then turn on its glow
                newInteractable.InteractionGlowOn();

                if (currentInteractable.HasTask()) {
                    if (character.contextTaskUI != null) {
                        character.contextTaskUI.SetTask(currentInteractable.task);
                    }
                }
                // If we are pressing mouse down then do the interaction
                if (Input.GetButtonDown("Fire1")) {
                  // Do whatever the primary interaction of this interactable is.
                  currentInteractable.PrimaryInteraction(character);
                }
            }
        } 
        // Otherwise if we cant interact with anything but we were previously
        // interacting with something.
        else if (currentInteractable != null) {
            // Then turn off the glow of that thing
            currentInteractable.InteractionGlowOff();

            if (character.contextTaskUI != null) {
                character.contextTaskUI.RemoveTask();
            }

            // And if bring the mouse button up
            if (Input.GetButtonUp("Fire1")) {
              // Some item have a primary interaction off method, eg drop the
              // item after pickup. Therefore run this on mouse up.
              if (possibleInteractables.Contains(currentInteractable)) {
                possibleInteractables.Remove(currentInteractable);
              }
              currentInteractable.PrimaryInteractionOff(character);
              currentInteractable = null;
            }
        }
    }

    private Interactable ClosestInteractable() {
        float distance = float.PositiveInfinity;
        Interactable closestInteractable = possibleInteractables[0];
        foreach(Interactable interactable in possibleInteractables) {
            float interactableDistance = Vector3.Distance(transform.position, interactable.transform.position);
            if (interactableDistance < distance) {
                distance = interactableDistance;
                closestInteractable = interactable;
            }
        }
        return closestInteractable;
    }
 
     public void OnTriggerEnter(Collider collider){
        Interactable interactable = collider.GetComponent<Interactable>();
        if (interactable != null) {
            interactable.inRange = true;
            interactable.SetTaskGlow();
        }
     }

     public void OnTriggerExit(Collider collider){
        Interactable interactable = collider.GetComponent<Interactable>();
        if (interactable != null) {
            interactable.inRange = false;
            interactable.SetTaskGlow();
        }
     }

     public void OnInteractionConeEnter(Collider collider) {
        Interactable interactable = collider.GetComponent<Interactable>();
        if (interactable != null
            && interactable.gameObject.GetInstanceID() != gameObject.GetInstanceID()
        ) {
            possibleInteractables.Add(interactable);
        }
     } 
     
     public void OnInteractionConeExit(Collider collider) {
        Interactable interactable = collider.GetComponent<Interactable>();
        if (interactable != null) {
            possibleInteractables.Remove(interactable);
        }
     } 
}
