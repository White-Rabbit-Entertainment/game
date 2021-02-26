public class Pocketable : Interactable {
  public override void PrimaryInteraction(Character character) {
    character.AddItemToInventory(this);
    gameObject.SetActive(false);
  }
}
