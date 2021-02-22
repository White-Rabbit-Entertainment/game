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
            if (newInteractable != currentInteractable && currentInteractable != null) {
                currentInteractable.GlowOff();
            }
            currentInteractable = newInteractable;
            if (!(currentInteractable is Capturable && NetworkManager.instance.LocalPlayerPropertyIs<string>("Team", "Robber"))) {
                currentInteractable.GlowOn();
            }
        } else if (currentInteractable != null) {
            currentInteractable.GlowOff();
        }
        
        if (currentInteractable is PickUpable) {
            // Has interact button been pressed whilst interactable object is in front of player?
            if (Input.GetButtonDown("Fire1") && canInteract && currentHeldItem == null) {
                currentHeldItem = (PickUpable)currentInteractable;
                currentHeldItem.PickUp(pickupDestination);
            //on release mouse click drop item
            } else if (Input.GetButtonUp("Fire1") && currentHeldItem != null) { 
                currentHeldItem.PutDown();
                currentHeldItem = null;
            }
        }

        if (currentInteractable is MultiuseInteractable) {
            if (Input.GetButtonDown("Fire1") && canInteract && currentHeldItem == null) {
                ((MultiuseInteractable)currentInteractable).OnClick();
            }
        }

        if (currentInteractable is Capturable && NetworkManager.instance.LocalPlayerPropertyIs<string>("Team", "Seeker")) {
            // Has interact button been pressed whilst interactable object is in front of player?
            if (Input.GetButtonDown("Fire1") && canInteract && currentHeldItem == null) {
                ((Capturable)currentInteractable).Capture();
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
            canInteract = true;
        }
        else {
            canInteract = false;
        }
    }
}
