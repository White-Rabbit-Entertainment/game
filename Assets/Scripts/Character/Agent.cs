using UnityEngine;
using UnityEngine.AI;

public class Agent : Character {
  public override void Start() {
    team = Team.Agent;
    base.Start();
  }
  
  public override Vector3 Velocity() {
    return GetComponent<NavMeshAgent>().velocity;
  }
}
