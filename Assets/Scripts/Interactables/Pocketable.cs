using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

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
    SetItemDropConditions(position);
    View.RPC("SetItemDropConditions", RpcTarget.Others, position);
  }

  [PunRPC]
  public void SetItemDropConditionsRPC(Vector3 position) {
    gameObject.SetActive(true);
    transform.position = position;
    NetworkManager.instance.GetMe().GetComponent<ItemInteract>().RemovePossibleInteractable(this);
  }

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
    
    if (GetComponent<Rigidbody> != null) {
        GetComponent<Rigidbody>.velocity = Vector3.Zero;
        GetComponent<Rigidbody>.angularVelocity = Vector3.Zero;
    }
  }
 
  public override void OnParentTaskUncomplete() {
    // Move to random position
    View.RPC("SetItemDropConditions", RpcTarget.All, gameSceneManager.RandomNavmeshLocation(50f) + new Vector3(0f, 1f, 0f));
    task.Uncomplete();
  }
}
