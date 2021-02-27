public class Pocketable : Interactable {
  public Task requirementOf;

  public void IncludeInTask(Task task) {
    this.requirementOf = task;
  }

  public override void PrimaryInteraction(Character character) {
    character.AddItemToInventory(this);
    gameObject.SetActive(false);
  }
}
