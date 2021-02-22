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
    private bool canInteract = false;
    private PickUpable currentHeldItem;
    private Interactable currentInteractable;

    private void Start() {
        if (!photonView.IsMine) {
            Destroy(this);
        }
    }
 
    private void Update() {
        if (canInteract && currentHeldItem == null) {
            Interactable newInteractable = raycastFocus.collider.transform.GetComponent<Interactable>();
            // If we are already interacting with something but we are now
            // trying to interact with something new, then we need to disable
            // the other interaction (turn off its glow).
            if (newInteractable != currentInteractable && currentInteractable != null) {
                currentInteractable.GlowOff();
            }
            currentInteractable = newInteractable;

            // If we are able to interact with the new interactable then turn on its glow
            if (currentInteractable.CanInteract()) {
                currentInteractable.GlowOn();
            }
        } else if (currentInteractable != null) {
            currentInteractable.GlowOff();
        }
    
        if (Input.GetButtonDown("Fire1") && canInteract && currentHeldItem == null && currentInteractable.CanInteract()) {
            // If the interaction is pickup then we need to set where the item is going.
            if (currentInteractable is PickUpable) {
              ((PickUpable)currentInteractable).SetPickUpDestination(pickupDestination);
            }

            // Do whatever the primary interaction of this interactable is.
            currentInteractable.PrimaryInteraction();
        }
        
        if (Input.GetButtonUp("Fire1") && currentInteractable != null) {
            // Some item have a primary interaction off method, eg drop the
            // item after pickup. Therefore run this on mouse up.
            currentInteractable.PrimaryInteractionOff();
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
            canInteract = true;
        }
        else {
            canInteract = false;
        }
    }
}
