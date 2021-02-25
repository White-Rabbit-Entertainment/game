using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary><c>PickUpable</c> extends <c>Interactable</c> to allow the item to
/// be picked up and put down. </summary>
public abstract class PickUpable : Interactable {
    
  private Transform pickupDestination;
  public bool isPickedUp = false;
  public string playerAnimationTrigger = "PickUp";

  public override void PrimaryInteraction(Character character) {
    if (!isPickedUp) {
      character.PickUp(this);
    }
    PlayCharacterAnimation(character);
  }

  public override void PrimaryInteractionOff(Character character) {
    if (isPickedUp) {
      character.PutDown(this);
    }
  }

  /// <summary> Checks if the item is in a pickup destination, if so it is
  /// picked up.  </summary>
  public override bool CanInteract(Character character) {
    return !isPickedUp && !character.HasItem();
  }

  [PunRPC]
  public void SetItemPickupConditionsRPC() {
    isPickedUp = true;
    // Disable the box collider to prevent collisions whilst carrying item.
    // Also turn off gravity on item and freeze its Rigidbody.
    GetComponent<BoxCollider>().enabled = false;                                        
    GetComponent<Rigidbody>().useGravity = false;
  }
  
  public void SetItemPickupConditions() {
    GetComponent<PhotonView>().RPC("SetItemPickupConditionsRPC", RpcTarget.All);
    GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
  }
  
  [PunRPC]
  public void ResetItemConditionsRPC() {
    isPickedUp = false;
    GetComponent<BoxCollider>().enabled = true;
    GetComponent<Rigidbody>().useGravity = true;
  }
  
  public void ResetItemConditions() {
    GetComponent<PhotonView>().RPC("ResetItemConditionsRPC", RpcTarget.All);
    GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

    // Set velocity of box after it is putdown to the speed to the character moving it
    GetComponent<Rigidbody>().velocity = transform.parent.parent.GetComponent<CharacterController>().velocity/2;
  }
}
