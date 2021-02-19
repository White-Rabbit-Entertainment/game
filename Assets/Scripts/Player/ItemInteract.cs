using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
 
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
            currentInteractable = raycastFocus.collider.transform.GetComponent<Interactable>();
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
        if (Physics.Raycast(ray, out raycastFocus, maxInteractionDistance) && raycastFocus.collider.transform.tag == "interactable") {
            canInteract = true;
        }
        else {
            canInteract = false;
        }
    }
}
