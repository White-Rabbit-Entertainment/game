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
    public bool hasInteractedWithCurrentInteractble = false;
    private PlayableCharacter character;

    public SphereCollider itemCollider;

    public List<Interactable> interactablesInRange = new List<Interactable>();
    public List<Interactable> possibleInteractables = new List<Interactable>();

    void Start() {
        if (photonView != null && !photonView.IsMine) {
            Destroy(GetComponent<AudioListener>());
            Destroy(this);
        } else {
            character = GetComponent<PlayableCharacter>();
            itemCollider.enabled = true;
        }
    }

    void Update() {
        Interactable newInteractable = null; 
        // We can only interact with an item if the item is in reach and we are
        // not currently holding an item.
        newInteractable = character.HasItem() ? character.currentHeldItem : GetBestInteractable();
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

                // If we are pressing mouse down then do the interaction
                if (Input.GetButtonDown("Fire1")) {
                  // Do whatever the primary interaction of this interactable is.
                  currentInteractable.PrimaryInteraction(character);
                  hasInteractedWithCurrentInteractble = true;
                }
            }
        } 
        // If we cant interact with anything but we were previously
        // interacting with something or we were previously interacting with
        // something and we are now trying to do a mouse up.
        if (currentInteractable != null && (Input.GetButtonUp("Fire1") || newInteractable == null)) {
            Debug.Log($"Dropping {newInteractable}");
            // Then turn off the glow of that thing and do the interaction off
            currentInteractable.InteractionGlowOff();

            // If we havent interacted with the thing then we cannot uninteract
            if (hasInteractedWithCurrentInteractble) {
                currentInteractable.PrimaryInteractionOff(character);
            }

            currentInteractable = null;
            hasInteractedWithCurrentInteractble = false;
        }
    }

    public Interactable GetBestInteractable() {
        Debug.Log("Getting best interactable");
        float angle = float.PositiveInfinity;
        Interactable bestInteractable = null;
        foreach(Interactable interactable in possibleInteractables) {
            // Checks if the iteractable has been destroyed (eg player turns into ghost)
            if (interactable != null) { 
                RaycastHit hit;
                Vector3 direction = interactable.transform.position - cameraTransform.position;
                Physics.Raycast(cameraTransform.position, direction, out hit); 
                float interactableAngle = Vector3.Angle(cameraTransform.forward, direction);

                if (interactableAngle < angle && hit.collider != null && hit.collider.GetComponent<Interactable>() == interactable) {
                    angle = interactableAngle;
                    bestInteractable = interactable;
                }
            }
        }
        return bestInteractable;
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
            interactable.OnEnterPlayerRadius();
        }
     }

     public void OnTriggerExit(Collider collider){
        Interactable interactable = collider.GetComponent<Interactable>();
        if (interactable != null) {
            interactable.OnExitPlayerRadius();
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

     public void RemovePossibleInteractable(Interactable item) {
        possibleInteractables.Remove(item);
     }

     public void ClearInteractionOutline() {
         if (currentInteractable != null) {
             currentInteractable.InteractionGlowOff();
         }
     }
}
