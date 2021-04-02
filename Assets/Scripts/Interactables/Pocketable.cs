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
  }

  public override void PrimaryInteraction(Character character) {
    character.AddItemToInventory(this);
    base.PrimaryInteraction(character);

    // Enable the indicator for the real characters
    if (character == NetworkManager.instance.GetMe()) {
      if (task != null && task.parent != null && !task.parent.isCompleted) {
        task.parent.GetComponent<Interactable>().EnableTarget();
      }
    }
  }

  public override void OnTaskComplete(Character character) {
    // Take out of inventory
    character.RemoveItemFromInventory();
    character.GetComponent<ItemInteract>().RemovePossibleInteractable(this);
    gameObject.SetActive(false);
  }
  
  public override void OnTaskUncomplete() {
    // Move to random position
    transform.position = gameSceneManager.RandomNavmeshLocation(50f);
    gameObject.SetActive(true);
  }
}
