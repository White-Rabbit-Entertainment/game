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
    public string current_animation_state;
    public int frames = 0;
    public Vector3 targetVector;
    List<string> animationConditions = new List<string>() {
      "Talking","Dancing","Idle"
    };
    public float speed = 0.5f;
    Animator animator;

    private bool isGoalSet = false;
    private bool isGoalAchieved = false;
    private Interactable[] possibleGoals;
    private Interactable currentGoal;
    private NavMeshPath path;
    private bool pathSet = false;
    private float elapsed = 0f;

    // The empty gameobject which holds all the interactables
    public GameObject interactablesGameObject;

    void Start() {
        navMeshAgent = this.GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
        current_animation_state = "Idle";
    }

    private Interactable[] CheckEnvironment() {
      return interactablesGameObject.GetComponentsInChildren(Interactable);
    }

    private Interactable SetGoal(Interactable[] possGoals){
      System.Random r = new System.Random(System.Guid.NewGuid().GetHashCode());
      int randInteractable = r.Next(possGoals.Length);
      return possGoals[randInteractable];
    }

    private bool AttemptGoal(Interactable currGoal) {
      float distance = GetDistance(currGoal);
      if (distance > 3f) {
         // string possibleAction = get_animation_condition();
         // doIntermediaryAction(possibleAction);
         return false;
      }
      else {
        frames++;
        if(frames == 500){
          frames = 0;
          currGoal.PrimaryInteraction(navMeshAgent.gameObject.GetComponent<Agent>()); //interact with interactable
          Debug.Log("interact");
          return true;
        } else {
          return false;
        }
      }
    }

    private float GetDistance(Interactable currGoal){
      float dist = Vector3.Distance(currGoal.transform.position, transform.position);
      return dist;
    }

    private bool CalculateNewPath(Interactable currGoal) {

         NavMeshHit navHit;
         NavMesh.SamplePosition(currGoal.transform.position,out navHit,wanderRadius,-1);
         Vector3 x = new Vector3(navHit.position.x,transform.position.y,navHit.position.z);
         navMeshAgent.CalculatePath(x, path);
         print("New path calculated");
         if (path.status != NavMeshPathStatus.PathComplete) {
             return false;
         }
         else {
             return true;
         }
     }

    private void CalculatePath(Interactable currGoal){
      NavMesh.CalculatePath(transform.position, currGoal.transform.position, NavMesh.AllAreas, path);
      if(CalculateNewPath(currGoal)){
        pathSet = true;
        navMeshAgent.SetPath(path);
      } else {
        pathSet = false;
      }
    }

    void Update(){
      if (!isGoalSet) {
        possibleGoals = CheckEnvironment();
        if (possibleGoals.Length == 0){
          Debug.Log("no goals");
        }
        currentGoal = SetGoal(possibleGoals);
        isGoalSet = true;
        CalculatePath(currentGoal);
        if(!pathSet){
          isGoalSet = false;
        }
      }
      else {
        isGoalAchieved = AttemptGoal(currentGoal);
        if(isGoalAchieved){
          isGoalSet = false;
        }
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
