using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary><c>PickUpable</c> extends <c>Interactable</c> to allow the item to
/// be picked up and put down. </summary>
public abstract class PickUpable : Interactable {
    
  private Transform pickupDestination;
  public bool isPickedUp = false;

  public override void PrimaryInteraction(Character player) {
    Animator animator = player.GetComponentInChildren<Animator>();
    animator.SetTrigger("Pickup");
    PickUp(pickupDestination);
  }

  public override void PrimaryInteractionOff() {
    PutDown();
  }

  /// <summary> Checks if the item is in a pickup destination, if so it is
  /// picked up.  </summary>

  public override bool CanInteract(Character character) {
    return !isPickedUp && !character.HasItem();
  }

  /// <summary> Set the transform where the item will be once it it picked up.
  /// This is usually on the player which picks it up. </summary>
  public void SetPickUpDestination(Transform pickupDestination) {
    this.pickupDestination = pickupDestination;
  }

  /// <summary> Pickup item and freeze it on player. </summary>
  public void PickUp(Transform pickupDestination) {

      if(!isPickedUp) {
        // An item can only be moved by a player if they are the owner.
        // Therefore, give ownership of the item to the local player before
        // moving it.
        PhotonView view = GetComponent<PhotonView>();
        view.TransferOwnership(PhotonNetwork.LocalPlayer);
        view.RPC("SetItemPickupConditions", RpcTarget.All);
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

        // Move to players pickup destination.
        transform.position = pickupDestination.position;
        
        // Set the parent of the object to the pickupDestination so that it moves
        // with the player.
        transform.parent = pickupDestination;
      }
    }

    /// <summary> Drop item. </summary>
    public void PutDown() {
      if(isPickedUp) {
        GetComponent<PhotonView>().RPC("ResetItemConditions", RpcTarget.All);
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        // Set velocity of box after it is putdown to the speed to the character moving it
        GetComponent<Rigidbody>().velocity = transform.parent.parent.GetComponent<CharacterController>().velocity/2;
        
        transform.parent = GameObject.Find("/Environment").transform;
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
