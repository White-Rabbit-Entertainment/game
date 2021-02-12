using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class ItemPickup : MonoBehaviour {

     public Transform pickupDestination;
     [SerializeField] private Transform cameraTransform;

     public float throwForce = 200f;
     public float maxInteractionDistance = 100f;
 
     private RaycastHit raycastFocus;
     private bool canInteract = false;

     private void Start() {
       List<Camera> cameras = gameObject.GetComponentsInChildrenOfAsset<Camera>();
     }
 
 
     private void Update() {
         // Has interact button been pressed whilst interactable object is in front of player?
         if (Input.GetButtonDown("Fire1") && canInteract) {
             Interactable interactComponent = raycastFocus.collider.transform.GetComponent<Interactable>();
 
             if (interactComponent != null) {
                 // Perform object's interaction
                 interactComponent.Pickup(pickupDestination);
             }
         }
 
         // Has action button been pressed whilst interactable object is in front of player?
         // if (Input.GetButtonDown("Fire3") && canInteract == true) {
         //     IInteractable interactComponent = raycastFocus.collider.transform.GetComponent<IInteractable>();
 
         //     if (interactComponent != null) {
         //         // Perform object's action
         //         interactComponent.Action(this);
         //     }
         // }
     }
 
     private void FixedUpdate() {
         Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
 
         // Is interactable object detected in front of player?
         if (Physics.Raycast(ray, out raycastFocus, maxInteractionDistance) && raycastFocus.collider.transform.tag == "Interactable") {
            canInteract = true;
         }
         else {
            canInteract = false;
         }
     }
 }
