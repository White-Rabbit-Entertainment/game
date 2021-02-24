using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
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
  }
  
  public void PutDown(PickUpable item) {
    currentHeldItem = null;
  }
}
