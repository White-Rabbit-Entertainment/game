using Photon.Pun;

public class Pocketable : Interactable {
  public Task requirementOf;

  [PunRPC]
  public void SetItemPocketConditions() {
    gameObject.SetActive(false);
  }

  public void IncludeInTask(Task task) {
    this.requirementOf = task;
  }

  public override void PrimaryInteraction(Character character) {
    character.AddItemToInventory(this);
  }
}
