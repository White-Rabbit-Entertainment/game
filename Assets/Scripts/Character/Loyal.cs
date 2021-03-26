public class Loyal : PlayableCharacter {
  public override void Start() {
    base.Start();
    team = Team.Loyal;
    canTask = true;
  }
}
