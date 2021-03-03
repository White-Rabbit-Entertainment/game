using Photon.Pun;

public class Pocketable : Interactable {

  [PunRPC]
  public void SetItemPocketConditions() {
    gameObject.SetActive(false);
  }

  public override void PrimaryInteraction(Character character) {
    character.AddItemToInventory(this);
    base.PrimaryInteraction(character);
  }
}
