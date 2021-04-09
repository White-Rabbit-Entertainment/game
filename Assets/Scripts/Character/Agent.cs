using UnityEngine;
using UnityEngine.AI;

public class Agent : Character {
  protected override void Start() {
    team = Team.Agent;
  }

  public override Vector3 Velocity() {
    return GetComponent<NavMeshAgent>().velocity;
  }
}
