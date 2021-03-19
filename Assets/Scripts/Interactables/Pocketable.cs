using Photon.Pun;
using UnityEngine;

public class Pocketable : Interactable {

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
  }
}
