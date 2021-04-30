public class Ghost : PlayableCharacter {
  protected override void Start() {
    team = Team.Ghost;
  }

  public override void Freeze() {
    // When GHOST movement is added this will need to change
    base.Freeze();
  }
}
                                         
