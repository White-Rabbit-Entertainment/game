using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
  public GameObject pickupDestination; 
  public PickUpable currentHeldItem; 

  public Team team;
  public bool HasItem() {
    return currentHeldItem != null; 
  }

  public virtual void Start() {
  }
}
