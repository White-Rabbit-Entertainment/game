using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
 
/// <summary><c>ItemInteract</c> is the class which defines the behaviour for
/// how a player interacts with an <c>Interactable</c>. 
/// E.g. Defines when to turn on glow and when to pickup a PickUpable
public class ItemInteract : MonoBehaviourPun {

    public Transform pickupDestination;
    public float maxInteractionDistance = 2f;
 
    [SerializeField] private Transform cameraTransform;

    private RaycastHit raycastFocus;
    private bool interactableInRange = false;
    private PickUpable currentHeldItem;
    private Interactable currentInteractable;
    private void Start() {
        if (!photonView.IsMine) {
            Destroy(this);
        }
    }
 
    private void Update() {
        
        // We can only interact with an item if the item is in reach and we are
        // not currently holding an item.
        bool canInteract = interactableInRange && currentHeldItem == null;

        // If we are able to interact with stuff
        if (canInteract) {
            Interactable newInteractable = raycastFocus.collider.transform.GetComponent<Interactable>();
            // If we are already interacting with something but we are now
            // trying to interact with something new, then we need to disable
            // the other interaction (turn off its glow).
            if (newInteractable != currentInteractable && currentInteractable != null) {
                currentInteractable.GlowOff();
            }
            currentInteractable = newInteractable;
            
            if (currentInteractable != null && currentInteractable.CanInteract()) {
                // If we are able to interact with the new interactable then turn on its glow
                currentInteractable.GlowOn();

                // If we are pressing mouse down then do the interaction
                if (Input.GetButtonDown("Fire1")) {
                  // If the interaction is pickup then we need to set where the item is going.
                  if (currentInteractable is PickUpable) {
                    currentHeldItem = (PickUpable)currentInteractable;
                    currentHeldItem.SetPickUpDestination(pickupDestination);
                  }
                  // Do whatever the primary interaction of this interactable is.
                  currentInteractable.PrimaryInteraction(gameObject);
                }
            }
        } 
        // Otherwise if we cant interact with anything but we were previously
        // interacting with something.
        else if (currentInteractable != null) {
            // Then turn off the glow of that thing
            currentInteractable.GlowOff();

            // And if bring the mouse button up
            if (Input.GetButtonUp("Fire1")) {
              if (currentInteractable is PickUpable) {
                currentHeldItem = null;
              }
              // Some item have a primary interaction off method, eg drop the
              // item after pickup. Therefore run this on mouse up.
              currentInteractable.PrimaryInteractionOff();
              currentInteractable = null;
            }
        }
    }
 
    private void FixedUpdate() {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
 
        // Is interactable object detected in front of player?
        if (
          // Fire a ray out and see if we hit anything within a max distance
              Physics.Raycast(ray, out raycastFocus, maxInteractionDistance) 
          // If we hit something that is not interactalbe then it doesnt count 
          &&  raycastFocus.collider.transform.GetComponent<Interactable>() != null
          // If we hit ourselves then it also doesnt count 
          &&  raycastFocus.collider.gameObject.GetInstanceID() != gameObject.GetInstanceID()
        ) {
            interactableInRange = true;
        }
        else {
            interactableInRange = false;
        }
    }
}
