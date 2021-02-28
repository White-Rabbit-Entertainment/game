﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class Character : MonoBehaviour
{
  public Transform pickupDestination;
  public PickUpable currentHeldItem;

  public Team team;
  public bool HasItem() {
    return currentHeldItem != null;
  }

  public virtual void Start() {
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

    item.transform.parent = GameObject.Find("/Environment/Interactables").transform;
  }

  public virtual Vector3 Velocity() {
    return GetComponent<CharacterController>().velocity;
  }
}
