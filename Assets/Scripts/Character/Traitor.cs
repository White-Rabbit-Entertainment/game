public class Traitor : PlayableCharacter {

  public bool hasPoison = true;
  public PoisonUI poisonUI;

  public override void Start() {
    base.Start();
    team = Team.Traitor;
    canTask = false;
  }  

  public void UsePoison() {
    poisonUI.UsePoison();
    hasPoison = false;
  }
}
