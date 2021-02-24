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
    PlayCharacterAnimation(character);
    PickUp(character);
  }

  public override void PrimaryInteractionOff(Character character) {
    PutDown(character);
  }

  /// <summary> Checks if the item is in a pickup destination, if so it is
  /// picked up.  </summary>
  public override bool CanInteract(Character character) {
    return !isPickedUp && !character.HasItem();
  }

  /// <summary> Pickup item and freeze it on player. </summary>
  public void PickUp(Character character) {

      if(!isPickedUp) {
        // An item can only be moved by a player if they are the owner.
        // Therefore, give ownership of the item to the local player before
        // moving it.
        PhotonView view = GetComponent<PhotonView>();
        view.TransferOwnership(PhotonNetwork.LocalPlayer);
        view.RPC("SetItemPickupConditions", RpcTarget.All);
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

        // Move to players pickup destination.
        transform.position = character.pickupDestination.position;
        
        // Set the parent of the object to the pickupDestination so that it moves
        // with the player.
        transform.parent = character.pickupDestination;

        // Set the character's currentHeldItem to this item
        character.PickUp(this);
      }
    }

    /// <summary> Drop item. </summary>
    public void PutDown(Character character) {
      if(isPickedUp) {
        GetComponent<PhotonView>().RPC("ResetItemConditions", RpcTarget.All);
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        // Set velocity of box after it is putdown to the speed to the character moving it
        GetComponent<Rigidbody>().velocity = transform.parent.parent.GetComponent<CharacterController>().velocity/2;
        
        transform.parent = GameObject.Find("/Environment").transform;
        character.PutDown(this);
      }
    }
  
    [PunRPC]
    public void SetItemPickupConditions() {
      isPickedUp = true;
      // Disable the box collider to prevent collisions whilst carrying item.
      // Also turn off gravity on item and freeze its Rigidbody.
      GetComponent<BoxCollider>().enabled = false;                                        
      GetComponent<Rigidbody>().useGravity = false;
    }
    
    [PunRPC]
    public void ResetItemConditions() {
      isPickedUp = false;
      GetComponent<BoxCollider>().enabled = true;
      GetComponent<Rigidbody>().useGravity = true;
    }
    
}
