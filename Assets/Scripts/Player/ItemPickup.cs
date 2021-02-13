using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
 
public class ItemPickup : MonoBehaviourPun {

    public Transform pickupDestination;
    public float maxInteractionDistance = 5f;
 
    [SerializeField] private Transform cameraTransform;

    private RaycastHit raycastFocus;
    private bool canInteract = false;
    private Interactable currentItem;

    private void Start() {
        if (!photonView.IsMine) {
            Destroy(this);
        }
    }
 
    private void Update() {
        
        // Has interact button been pressed whilst interactable object is in front of player?
        if (Input.GetButtonDown("Fire1") && canInteract && currentItem == null) {
            currentItem = raycastFocus.collider.transform.GetComponent<Interactable>();
            currentItem.PickUp(pickupDestination);

        } else if (Input.GetButtonUp("Fire1") && currentItem != null) {
            currentItem.PutDown();
            currentItem = null;
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
