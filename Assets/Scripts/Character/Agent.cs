using UnityEngine;
using UnityEngine.AI;

public class Agent : Character {

  //Start sets the characters team to Agent
  protected override void Start() {
    team = Team.Agent;
  }

  //Getter for the velocity of the nav mesh agent
  public override Vector3 Velocity() {
    return GetComponent<NavMeshAgent>().velocity;
  }
}
