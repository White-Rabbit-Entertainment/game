public class Traitor : PlayableCharacter {

  public bool hasPoison = true;

  public override void Start() {
    team = Team.Traitor;
    base.Start();
  }
}
