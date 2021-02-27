using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Animator))]
public abstract class Character : MonoBehaviour
{
  public Transform pickupDestination; 
  public PickUpable currentHeldItem; 
  public List<Pocketable> pocketedItems;
  public InventoryUI inventoryUI;

  public Team team;
  public bool HasItem() {
    return currentHeldItem != null; 
  }

  public bool HasItem(Interactable item) {
    if (item is PickUpable && currentHeldItem == (PickUpable)item) {
      return true;
    }
    if (item is Pocketable && pocketedItems.Contains((Pocketable)item)) {
      return true;
    }
    return false; 
  }

  public virtual void Start() {
    pocketedItems = new List<Pocketable>();
  }

  public void PickUp(PickUpable item) {
    currentHeldItem = item;
    // An item can only be moved by a player if they are the owner.
    // Therefore, give ownership of the item to the local player before
    // moving it.
    PhotonView view = item.GetComponent<PhotonView>();
    view.TransferOwnership(PhotonNetwork.LocalPlayer);

    item.SetItemPickupConditions();

    // Move to players pickup destination.
    item.transform.position = pickupDestination.position;
    
    // Set the parent of the object to the pickupDestination so that it moves
    // with the player.
    item.transform.parent = pickupDestination;
  }
  
  public void PutDown(PickUpable item) {
    currentHeldItem = null;
    item.ResetItemConditions();

    item.transform.parent = GameObject.Find("/Environment").transform;
  }

  public void AddItemToInventory(Pocketable item) {
    pocketedItems.Add(item);
    inventoryUI.AddItem(item);
    item.GetComponent<PhotonView>().RPC("SetItemPocketConditions", RpcTarget.All);
  }

  public List<Pocketable> GetItemsInInventory() {
    return pocketedItems;
  }
}
