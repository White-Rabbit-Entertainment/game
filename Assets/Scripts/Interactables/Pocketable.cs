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

  [PunRPC]
  public void SetItemDropConditions(Vector3 position) {
    gameObject.SetActive(true);
    transform.position = position;
    NetworkManager.instance.GetMe().GetComponent<ItemInteract>().RemovePossibleInteractable(this);
    SetTaskGlow();
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
  }
 
  public override void OnParentTaskUncomplete() {
    // Move to random position
    View.RPC("SetItemDropConditions", RpcTarget.All, gameSceneManager.RandomNavmeshLocation(50f));
    task.Uncomplete();
  }
}
