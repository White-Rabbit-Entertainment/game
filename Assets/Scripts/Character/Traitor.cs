public class Traitor : Character {

  public bool hasPoison = true;

  public override void Start() {
    team = Team.Traitor;
    base.Start();
  }
}
