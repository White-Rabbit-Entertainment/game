using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;


public class AgentController : MonoBehaviourPun
{
    [SerializeField]
    NavMeshAgent navMeshAgent;
    public float wanderRadius = 30f;
    
    // public string currentAnimationState;
    // List<string> animationConditions = new List<string>() {
    //   "Talking","Dancing","Idle"
    // };
    // public float speed = 0.5f;
    Animator animator;

    private Interactable currentGoal;
    private NavMeshPath path;
    private bool pathSet = false;
    private bool goalInProgress = false;

    // The empty gameobject which holds all the interactables
    public GameObject interactablesGameObject;
    public List<Interactable> interactables;

    void Start() {
        navMeshAgent = this.GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
        interactables = new List<Interactable>(interactablesGameObject.GetComponentsInChildren<Interactable>());
        animator = this.GetComponent<Animator>();
        // currentAnimationState = "Idle";
    }

    private Interactable SetGoal(){
      if (interactables.Count == 0) {
        return null;
      }

      System.Random r = new System.Random();
      int randIndex = r.Next(interactables.Count);
  
      Interactable newInteractable = interactables[randIndex];
      if (!newInteractable.CanInteract(GetComponent<Agent>())) {
        return null;
      }
      return newInteractable;
    }

    private IEnumerator CompleteGoal() {
      yield return new WaitForSeconds(5);
      currentGoal.PrimaryInteraction(GetComponent<Agent>()); //interact with interactable
      currentGoal = null;
      goalInProgress = false;
    }

    private float GetDistance(Interactable currGoal){
      return Vector3.Distance(currGoal.transform.position, transform.position);
    }

    private void CalculatePath(Interactable currentGoal){
      NavMeshHit navHit;
      NavMesh.SamplePosition(currentGoal.transform.position, out navHit, wanderRadius,-1);
      Vector3 targetLocation = new Vector3(navHit.position.x,transform.position.y,navHit.position.z);
      navMeshAgent.CalculatePath(targetLocation, path);
      navMeshAgent.SetPath(path);
    }

    void Update(){
      // Set the walking speed for the animator
      animator.SetFloat("Walking", navMeshAgent.velocity.magnitude);

      if (currentGoal == null) {
        currentGoal = SetGoal();
      } else if(path.status != NavMeshPathStatus.PathComplete) {
        CalculatePath(currentGoal);
      } else if (!(GetDistance(currentGoal) > 3f) && !goalInProgress) {
        goalInProgress = true;
        StartCoroutine(CompleteGoal());
      }
    }

    // public string get_animation_condition(){
    //   // return animation_conditions.PickRandom();
    //   System.Random r = new System.Random(System.Guid.NewGuid().GetHashCode());
    //   int randNum = r.Next(animationConditions.Count);
    //   return (string)animationConditions[randNum];
    // }

    // public Vector3 RandomWanderPoint()
    // {
    //   Vector3 randomPoint = (Random.insideUnitSphere * wanderRadius) + transform.position;
    //   NavMeshHit navHit;
    //   NavMesh.SamplePosition(randomPoint,out navHit,wanderRadius,-1);
    //   return new Vector3(navHit.position.x,transform.position.y,navHit.position.z);
    // }


}
