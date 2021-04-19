using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Photon.Pun;

public class ConsumablePlaceable : Placeable {
  
  GameSceneManager gameSceneManager;

  public override void Start() {
    base.Start();
    gameSceneManager = GameObject.Find("/GameSceneManager").GetComponent<GameSceneManager>();
  }

  public override void OnParentTaskComplete(Character character = null) {
    // Take out of inventory
    if (character != null) {
      character.RemoveItemFromInventory(false);
    }
    gameObject.SetActive(false);
  }
 
  public override void OnParentTaskUncomplete() {
    // Move to random position
    View.RPC("SetItemDropConditions", RpcTarget.All, gameSceneManager.RandomNavmeshLocation(50f));
    task.Uncomplete();
  }
}