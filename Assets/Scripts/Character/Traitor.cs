public class Traitor : PlayableCharacter {

  protected override void Start() {
    base.Start();
    team = Team.Traitor;
  }  
}
