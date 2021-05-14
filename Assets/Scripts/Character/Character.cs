﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(Animator))]
public abstract class Character : MonoBehaviour {
  public Transform pickupDestination; 

  public Sabotageable currentFixingItem;
  public Pickupable currentHeldItem; 
  public Pocketable pocketedItem;
  
  public InventoryUI inventoryUI;

  public Team team;
  public Team startingTeam; 

  public PlayerInfo playerInfo;
    
  public Player Owner {
      get { return GetComponent<PhotonView>().Owner; }
    }
  
  public PhotonView View {
      get { return GetComponent<PhotonView>(); }
    }

  public Color Color {
      get { return playerInfo.color; } 
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
    item.View.TransferOwnership(PhotonNetwork.LocalPlayer);
    item.SetItemPickupConditions();

    // Move to players pickup destination.
    item.transform.position = pickupDestination.position;
    
    // Set the parent of the object to the pickupDestination so that it moves
    // with the player.
    item.transform.parent = pickupDestination;
  }

    public virtual void Fix(Sabotageable item) {
    currentFixingItem = item;
    if (currentFixingItem != null) Debug.Log("fixing");
  }

    public virtual void StopFix(Sabotageable item) {
    currentFixingItem = null;
  }  
  public virtual void PutDown(Pickupable item) {
    if (HasItem(item)) {
      Debug.Log($"Character putting down {gameObject}");
      currentHeldItem = null;
      item.ResetItemConditions(this);
      Debug.Log($"Reset item conditions");
      item.transform.parent = GameObject.Find("/Environment").transform;
      Debug.Log($"Put the item in the game scene");
    }
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
    if (pocketedItem == null) return;
    if (resetConditions) {
      pocketedItem.SetItemDropConditions(transform.position);
      if (pocketedItem.task != null && pocketedItem.task.parent != null) {
        pocketedItem.task.parent.GetComponent<Interactable>().DisableTaskMarker();
      }
    }
    if (pocketedItem.task != null && pocketedItem.task.IsRequired()) {
      pocketedItem.task.Uncomplete();
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
    return playerInfo != null;
  } 

  [PunRPC]
  public void AssignColor (string assetPath) {
      GameObject playerPrefab = PlayerInfo.GetPrefab(assetPath);
      playerInfo = playerPrefab.GetComponent<PlayerInfo>();
      Debug.Log($"Ghost prefab is {playerInfo.ghostPrefab}");

      // Dont spawn your own body
      GameObject body;
      if (Owner != PhotonNetwork.LocalPlayer) {
        body = Instantiate(playerPrefab, new Vector3(0,0,0), Quaternion.identity);
        body.transform.parent = transform; // Sets the parent of the body to the player
        body.transform.position = transform.position + new Vector3(0,-1.2f, -0.2f);
        GetComponent<Votable>().outline = body.GetComponent<Outline>();
      }

      GetComponent<Animator>().avatar = playerInfo.avatar;
  }
}
