using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;


public class NpcMove : MonoBehaviourPun
{
    [SerializeField]
    // Transform _destination;
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
    private PickUpable currentHeldItem;

    public Transform pickupDestination;


    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = this.GetComponent<NavMeshAgent>();
        // animator = this.GetComponent<Animator>();
        path = new NavMeshPath();
        current_animation_state = "Idle";
        // current_animation_state = idle;
    }

    // possibleGoals = checkEnvironment(); returns interactables in the environment
    // setGoal(possibleGoals); might be to get to somewhere, or interact with an interactable
    // in a loop, maybe achieve goal or perhaps with probability do a set of actions in an intemediate stage to doing the goal.
    // if goal achieved, checkEnvironment() and start again.

    private Interactable[] checkEnvironment() {
      Interactable[] interactables = GameObject.FindObjectsOfType<Interactable>();
      return interactables;
    }

    private Interactable setGoal(Interactable[] possGoals){
      System.Random r = new System.Random(System.Guid.NewGuid().GetHashCode());
      int randInteractable = r.Next(possGoals.Length);
      return (Interactable)possGoals[randInteractable];
    }

    private bool attemptGoal(Interactable currGoal) {
      //check how far you are from object
      //if not interactable radius, then walk towards object, return false
      //else interact with interactable, return true
      float distance = getDistance(currGoal);
      if (distance > 3f) {
         // walkPath(currGoal);
         // string possibleAction = get_animation_condition();
         // doIntermediaryAction(possibleAction);
         return false;
      }
      else {
        frames++;
        if(frames == 500){
          frames = 0;
          interactWithInteractable(currGoal);
          Debug.Log("interact");
          return true;
        } else {
          return false;
        }

        // interactWithInteractable(currGoal);
        // return true;
      }
    }

    // private void doIntermediaryAction(string action){
    //
    // }

    private float getDistance(Interactable currGoal){
      float dist = Vector3.Distance(currGoal.transform.position, transform.position);
      return dist;
    }

    // private void walkPath(Interactable currGoal){
    //
    // }

    private void interactWithInteractable(Interactable currGoal){
      if (currGoal is PickUpable) {
        currentHeldItem = (PickUpable)currGoal;
        currentHeldItem.SetPickUpDestination(pickupDestination);
      }
      currGoal.PrimaryInteraction(navMeshAgent.gameObject);
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
         // return true;
     }

    private void calculatePath(Interactable currGoal){
      NavMesh.CalculatePath(transform.position, currGoal.transform.position, NavMesh.AllAreas, path);
      if(CalculateNewPath(currGoal)){
        pathSet = true;
        navMeshAgent.SetPath(path);
      } else {
        pathSet = false;
      }
    }
    // every frame:
    // - if !isGoalSet
    // - then checkEnvironment and setGoal
    // - if isGoalSet
    // - then perhaps do goal else choose from {idle, randomWonder, do non interactive animation}
    // - if goal achieved then set isGoalSet to false.
    // - let the goal be to 1) get to the interactable 2) interact with interactable


    void Update(){
      if (!isGoalSet) {
        possibleGoals = checkEnvironment();
        if (possibleGoals.Length == 0){
          Debug.Log("no goals");
        }
        currentGoal = setGoal(possibleGoals);
        isGoalSet = true;
        calculatePath(currentGoal);
        if(!pathSet){
          isGoalSet = false;
        }
      }
      else {
        isGoalAchieved = attemptGoal(currentGoal);
        if(isGoalAchieved){
          isGoalSet = false;
        }
      }
    }





    // void Update()
    // {
    //   frames++;
    //   if(frames % 30 == 0 || transform.position == targetVector)
    //   {
    //     string anim_cond = get_animation_condition();
    //     //animator.ResetTrigger(current)
    //     //animator.SetTrigger(anim_cond)
    //     if(anim_cond != current_animation_state){
    //       animator.ResetTrigger(current_animation_state);
    //       animator.SetTrigger(anim_cond);
    //       current_animation_state = anim_cond;
    //       if (anim_cond == "Walking"){
    //         animator.SetBool("Walking", true);
    //       } else if(anim_cond == "Idle"){
    //         animator.SetBool("Walking",false);
    //       } else {
    //
    //       };
    //       // animator.SetInteger("condition",current_animation_state);
    //     }
    //   }
    //   SetDestination(current_animation_state);
    // }

    // Update is called once per frame
    // private void SetDestination(string anim_cond)
    // {
    //     if(_destination != null){
    //       if(anim_cond == "Walking"){
    //         // System.Console.WriteLine("walking...");
    //         targetVector = RandomWanderPoint();
    //       }
    //       // Vector3 targetVector = _destination.transform.position;
    //       else{
    //         targetVector = transform.position;
    //       };
    //       _navMeshAgent.speed = speed;
    //       _navMeshAgent.SetDestination(targetVector);
    //     }
    // }
    //
    public string get_animation_condition(){
      // return animation_conditions.PickRandom();
      System.Random r = new System.Random(System.Guid.NewGuid().GetHashCode());
      int randNum = r.Next(animationConditions.Count);
      return (string)animationConditions[randNum];
    }

    // public int get_animation_condition(){
    //   // return animation_conditions.PickRandom();
    //   System.Random r = new System.Random(System.Guid.NewGuid().GetHashCode());
    //   int randNum = r.Next(0,2);
    //   return randNum;
    // }

    // public Vector3 RandomWanderPoint()
    // {
    //   Vector3 randomPoint = (Random.insideUnitSphere * wanderRadius) + transform.position;
    //   NavMeshHit navHit;
    //   NavMesh.SamplePosition(randomPoint,out navHit,wanderRadius,-1);
    //   return new Vector3(navHit.position.x,transform.position.y,navHit.position.z);
    // }


}
