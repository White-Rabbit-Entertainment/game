﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Animator))]
public abstract class Character : MonoBehaviour {
  public Transform pickupDestination; 
  public Pickupable currentHeldItem; 
  public List<Pocketable> pocketedItems;
  public InventoryUI inventoryUI;

  public bool canTask;

  public Team team;

  public RoleInfo roleInfo;
    
  public bool HasItem() {
    return currentHeldItem != null; 
  }

  public bool HasItem(Interactable item) {
    if (item is Pickupable && currentHeldItem == (Pickupable)item) {
      return true;
    }
    if (item is Pocketable && pocketedItems.Contains((Pocketable)item)) {
      return true;
    }
    return false; 
  }

  public virtual void Start() {
    canTask = false;
    pocketedItems = new List<Pocketable>();
  }

  public void PickUp(Pickupable item) {
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
  
  public void PutDown(Pickupable item) {
    currentHeldItem = null;
    item.ResetItemConditions(this);

    item.transform.parent = GameObject.Find("/Environment").transform;
  }

  public void AddItemToInventory(Pocketable item) {
    pocketedItems.Add(item);
    if (!(this is Agent)) {
      inventoryUI.AddItem(item);
    }
    item.GetComponent<PhotonView>().RPC("SetItemPocketConditions", RpcTarget.All);
  }

  public List<Pocketable> GetItemsInInventory() {
    return pocketedItems;
  }

  public virtual Vector3 Velocity() {
    return GetComponent<CharacterController>().velocity;
  }

  [PunRPC]
  public void AssignRole (Role role) {
      string prefabName = role.ToString();
      Debug.Log("Loading in");
      Debug.Log(prefabName);
      GameObject prefab = (GameObject)Resources.Load("Roles/" + prefabName, typeof(GameObject));
      GameObject body = Instantiate(prefab, new Vector3(0,0,0), Quaternion.identity);
      body.transform.parent = transform; // Sets the parent of the body to the player
      body.transform.position = transform.position + new Vector3(0,-1.2f, -0.2f);
      roleInfo = body.GetComponent<RoleInfo>();
  }
}
