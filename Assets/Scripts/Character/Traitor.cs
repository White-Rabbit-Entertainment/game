public class Traitor : PlayableCharacter {

  //Sets team and starting team to Traitor
  //starting team will be used to remember the 
  //player's original team if they become a ghost
  protected override void Start() {
    base.Start();
    team = Team.Traitor;
    startingTeam = team;
  }  
}
