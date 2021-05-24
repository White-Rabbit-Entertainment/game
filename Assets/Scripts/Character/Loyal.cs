public class Loyal : PlayableCharacter {
  
  //Sets team and starting team to Loyal
  //starting team will be used to remember the 
  //player's original team if they become a ghost
  protected override void Start() {
    base.Start();
    team = Team.Loyal;
    startingTeam = team;
  }

  //Enables the marker for player's current task
  public void EnableTaskMarker() {
    assignedSubTask.TaskInteractable.EnableTaskMarker();
  }

  //Disables the marker for player's current task
  public void DisableTaskMarker() {
    assignedSubTask.TaskInteractable.DisableTaskMarker();
  }
}
