using System.Collections;
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

  //Start function defined to be overriden by sub classes
  protected virtual void Start() {}

  //Returns true if player is currently holding an item
  public bool HasItem() {
    return currentHeldItem != null;
  }

  //Returns true if the player is holding the given item
  //or if they have the given item in their inventory
  public bool HasItem(Interactable item) {
    //Check if item is held by player
    if (item is Pickupable && currentHeldItem == (Pickupable)item) {
      return true;
    }
    //Check if item is in player's inventory
    if (item is Pocketable && pocketedItem == (Pocketable)item) {
      return true;
    }
    return false; 
  }


  //Picks up a pickupable item
  public virtual void Pickup(Pickupable item) {

    currentHeldItem = item;
    // An item can only be moved by a player if they are the owner.
    // Therefore, give ownership of the item to the local player before
    // moving it.
    item.View.TransferOwnership(PhotonNetwork.LocalPlayer);
    item.SetItemPickupConditions();

    // Move item to player's pickup destination.
    item.transform.position = pickupDestination.position;
    
    // Set the parent of the object to the pickupDestination so that it moves
    // with the player.
    item.transform.parent = pickupDestination;
  }

  //Puts down a pickupable item if the player is currently holding it
  public virtual void PutDown(Pickupable item) {
    //if the player is holding the given item
    //reset the item conditions and then unparent the item
    if (HasItem(item)) {
      currentHeldItem = null;
      item.ResetItemConditions(this);
      item.transform.parent = GameObject.Find("/Environment").transform;
    }
  }

  //Starts the player fixing a given sabotageable
  public virtual void Fix(Sabotageable item) {
    currentFixingItem = item;
  }

  //Ends the player fixing a given sabotageable
  public virtual void StopFix(Sabotageable item) {
    currentFixingItem = null;
  }

  //Puts a pocketable item in the player's inventory
  public void AddItemToInventory(Pocketable item) {

    //If player currently has an item in their inventory, remove it
    if (pocketedItem != null) {
      RemoveItemFromInventory();
    }

    //Set new pocketed item
    pocketedItem = item;

    //Handle UI for playable characters
    if (!(this is Agent)) {
      inventoryUI.AddItem(item);
    }

    item.GetComponent<PhotonView>().RPC("SetItemPocketConditions", RpcTarget.All);
  }

  //Removes the current item from the player's inventory
  public void RemoveItemFromInventory(bool resetConditions = true) {

    //If player does not currently have an item in their inventory, exit the function
    if (pocketedItem == null) return;

    //If resetConditions is true then we drop the item next to the player
    //otherwise we simply remove the item from the players inventory
    if (resetConditions) {
      pocketedItem.SetItemDropConditions(transform.position);

      //if the item has a sub task then disable the task marker
      //for the item with the master task
      if (pocketedItem.task != null && pocketedItem.task.parent != null) {
        pocketedItem.task.parent.GetComponent<Interactable>().DisableTaskMarker();
      }
    }

    //Uncomplete the item's task if there was one
    if (pocketedItem.task != null && pocketedItem.task.IsRequired()) {
      pocketedItem.task.Uncomplete();
    }

    //handle UI for playable characters
    if (!(this is Agent)) {
      inventoryUI.RemoveItem();
    }

    pocketedItem = null;
  }

  //Returns the item currently in the player's inventory
  public Pocketable GetItemsInInventory() {
    return pocketedItem;
  }

  //Returns character velocity
  public virtual Vector3 Velocity() {
    return GetComponent<CharacterController>().velocity;
  }

  //Returns whether the character has been spawned
  public bool Spawned() {
    return playerInfo != null;
  } 

  //Assigns a color and relevent body to the character
  [PunRPC]
  public void AssignColor (string assetPath) {

      //Get player info and prefab from assetPath
      GameObject playerPrefab = PlayerInfo.GetPrefab(assetPath);
      playerInfo = playerPrefab.GetComponent<PlayerInfo>();
      GameObject body;

      //if player is not the owner of the character then spawn the characters body
      if (Owner != PhotonNetwork.LocalPlayer) {

        //Create body and set position and parent to the character
        body = Instantiate(playerPrefab, new Vector3(0,0,0), Quaternion.identity);
        body.transform.parent = transform;
        body.transform.position = transform.position + new Vector3(0,-1.2f, -0.2f);

        //Set outline for votable script
        GetComponent<Votable>().outline = body.GetComponent<Outline>();
      }

      //Set avatar for animator
      GetComponent<Animator>().avatar = playerInfo.avatar;
  }
}
