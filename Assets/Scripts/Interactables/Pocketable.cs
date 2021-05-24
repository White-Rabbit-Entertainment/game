using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

// Pocketable extends Interactable to allow the item to
// be pocketed.
// When pocketed up it is added to they players inventory and its gameobject is
// disabled.
// Pocketed items are readded to the scene in a random position when their task
// is undone.
public class Pocketable : Interactable {
  
  public Texture image;
  private GameSceneManager gameSceneManager;

  public override void Start() {
    base.Start();
    gameSceneManager = GameObject.Find("/GameSceneManager").GetComponent<GameSceneManager>();
  }

  void Reset() {
    canBeMasterTask = false;
  }

  [PunRPC]
  public void SetItemPocketConditions() {
    gameObject.SetActive(false);
  }

  public void SetItemDropConditions(Vector3 position) {
    SetItemDropConditionsRPC(position);
    View.RPC("SetItemDropConditionsRPC", RpcTarget.Others, position);
  }

  [PunRPC]
  public void SetItemDropConditionsRPC(Vector3 position) {
    gameObject.SetActive(true);
    transform.position = position;
    NetworkManager.instance.GetMe().GetComponent<ItemInteract>().RemovePossibleInteractable(this);
  }

  // Place into inventory
  public override void PrimaryInteraction(Character character) {
    character.AddItemToInventory(this);
    base.PrimaryInteraction(character);
  }

  public override void OnParentTaskComplete(Character character = null) {
    // Take out of inventory
    if (character != null) {
      character.RemoveItemFromInventory(false);
    }
    gameObject.SetActive(false);
   
    // 0 all velocity to prevent bouncy things
    if (GetComponent<Rigidbody>() != null) {
        GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
        GetComponent<Rigidbody>().angularVelocity = new Vector3(0,0,0);
    }
  }

  // When the parent task is completed this task should also be uncompleted and
  // then the item should be re-placed in the scene at a random position (so
  // that it can be redone).
  public override void OnParentTaskUncomplete() {
    // Move to random position
    SetItemDropConditions(gameSceneManager.RandomNavmeshLocation());
    task.Uncomplete();
  }
}
