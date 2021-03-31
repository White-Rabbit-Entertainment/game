using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Pocketable : Interactable {
  
  public Texture image;

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
        task.parent.GetComponent<Interactable>().EnabledTarget();
      }
    }
  }
}
