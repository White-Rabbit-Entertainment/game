public class Traitor : PlayableCharacter {

  public override void Start() {
    base.Start();
    team = Team.Traitor;
    canTask = false;
  }  
}
