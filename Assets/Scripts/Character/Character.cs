using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(Animator))]
public abstract class Character : MonoBehaviour {
  public Transform pickupDestination; 
  public Pickupable currentHeldItem; 
  public Pocketable pocketedItem;
  
  public InventoryUI inventoryUI;

  public Team team;

  public RoleInfo roleInfo;

  public Player Owner {
      get { return GetComponent<PhotonView>().Owner; }
    }
  
  public PhotonView View {
      get { return GetComponent<PhotonView>(); }
    }

  protected virtual void Start() {}

  public bool HasItem() {
    return currentHeldItem != null; 
  }

  public bool HasItem(Interactable item) {
    if (item is Pickupable && currentHeldItem == (Pickupable)item) {
      return true;
    }
    if (item is Pocketable && pocketedItem == (Pocketable)item) {
      return true;
    }
    return false; 
  }

  public virtual void Pickup(Pickupable item) {
    currentHeldItem = item;
    // An item can only be moved by a player if they are the owner.
    // Therefore, give ownership of the item to the local player before
    // moving it.
    if (item.View != null) {
      item.View.TransferOwnership(PhotonNetwork.LocalPlayer);
      item.SetItemPickupConditions();
    } else {
      item.SetItemPickupConditionsRPC();
    }

    // Move to players pickup destination.
    item.transform.position = pickupDestination.position;
    
    // Set the parent of the object to the pickupDestination so that it moves
    // with the player.
    item.transform.parent = pickupDestination;
  }
  
  public virtual void PutDown(Pickupable item) {
    currentHeldItem = null;
    if (item.View != null) {
      item.ResetItemConditions(this);
    } else {
      item.ResetItemConditionsRPC();
    }
    item.transform.parent = GameObject.Find("/Environment").transform;
  }

  public void AddItemToInventory(Pocketable item) {
    if (pocketedItem != null) {
      RemoveItemFromInventory();
    }
    pocketedItem = item;
    if (!(this is Agent)) {
      inventoryUI.AddItem(item);
    }
    item.GetComponent<PhotonView>().RPC("SetItemPocketConditions", RpcTarget.All);
  }

  public void RemoveItemFromInventory(bool resetConditions = true) {

    if (resetConditions) {
      pocketedItem.GetComponent<PhotonView>().RPC("SetItemDropConditions", RpcTarget.All, transform.position);
    }
    if (pocketedItem.task != null && pocketedItem.task.IsRequired()) {
      pocketedItem.task.Uncomplete();
    }
    if (pocketedItem.task != null && pocketedItem.task.parent != null) {
      pocketedItem.task.parent.GetComponent<Interactable>().DisableTarget();
    }
    if (!(this is Agent)) {
      inventoryUI.RemoveItem();
    }
    pocketedItem = null;
  }

  public Pocketable GetItemsInInventory() {
    return pocketedItem;
  }

  public virtual Vector3 Velocity() {
    return GetComponent<CharacterController>().velocity;
  }

  public bool Spawned() {
    return roleInfo != null;
  } 

  [PunRPC]
  public void AssignRole (Role role) {
      string prefabName = role.ToString();
      GameObject prefab = (GameObject)Resources.Load("Roles/" + prefabName, typeof(GameObject));
      GameObject body = Instantiate(prefab, new Vector3(0,0,0), Quaternion.identity);
      body.transform.parent = transform; // Sets the parent of the body to the player
      body.transform.position = transform.position + new Vector3(0,-1.2f, -0.2f);
      roleInfo = body.GetComponent<RoleInfo>();
      GetComponent<Animator>().avatar = roleInfo.avatar;
  }
}
