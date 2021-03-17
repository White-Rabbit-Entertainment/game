public class Traitor : PlayableCharacter {

  public bool hasPoison = true;

  public override void Start() {
    base.Start();
    team = Team.Traitor;
    canTask = false;
  }
}
