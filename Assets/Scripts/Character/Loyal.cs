public class Loyal : PlayableCharacter {
  
  protected override void Start() {
    base.Start();
    team = Team.Loyal;
    startingTeam = team;
  }

  public void EnableTaskMarker() {
    assignedSubTask.TaskInteractable.EnableTaskMarker();
  }

  public void DisableTaskMarker() {
    assignedSubTask.TaskInteractable.DisableTaskMarker();
  }
}
