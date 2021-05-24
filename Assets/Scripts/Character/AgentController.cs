using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;


[RequireComponent(typeof(Agent))]
public class AgentController : MonoBehaviourPun {

    [SerializeField] NavMeshAgent navMeshAgent;
    [SerializeField] private Interactable currentGoal;

    public float maxInteractionDistance = 3f;
    Animator animator;

    private NavMeshPath path;
    private bool pathSet = false;
    private bool goalInProgress = false;
    private Agent character;

    public GameObject interactablesGameObject;
    public List<Interactable> interactables;

    void Start() {
        // Only the owner of the AI should control the AI
        animator = GetComponentInChildren<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        character = GetComponent<Agent>();

        if (!GetComponent<PhotonView>().IsMine) {
          Destroy(this);
        } else {
          path = new NavMeshPath();
          interactables = new List<Interactable>(interactablesGameObject.GetComponentsInChildren<Interactable>());
        }
    }

    void Update(){
        // Set the walking speed for the animator
        animator.SetFloat("Walking", navMeshAgent.velocity.magnitude);
        if (currentGoal == null) {
          currentGoal = SetGoal();
        } else if(path == null || path.status != NavMeshPathStatus.PathComplete) {
          try {
            CalculatePath(currentGoal);
          } catch {
            // If we failed to calculate a path then find a new goal
            currentGoal = null;
            path = null;
          }

        } else if (!(GetDistance(currentGoal) > maxInteractionDistance) && !goalInProgress) {
          if (currentGoal is Pickupable && ((Pickupable)currentGoal).isPickedUp) {
            currentGoal = null;
            path = null;
          } else {
            goalInProgress = true;
            StartCoroutine(CompleteGoal());
            StartCoroutine(EndGoal(currentGoal));
          }
        }
    }

    private Interactable SetGoal(){
      if (interactables.Count == 0) {
        return null;
      }

      System.Random r = new System.Random(System.Guid.NewGuid().GetHashCode());
      int randIndex = r.Next(interactables.Count);

      Interactable newInteractable = interactables[randIndex];
      if (!newInteractable.CanInteract(character)) {
        return null;
      }
      if (newInteractable.task != null) {
        Debug.Log("Found a task on an interactable");
        return null;
      }
      return newInteractable;
    }

    private IEnumerator CompleteGoal() {
      yield return new WaitForSeconds(5);
      if (currentGoal is Pickupable && !((Pickupable)currentGoal).isPickedUp) {
        currentGoal.PrimaryInteraction(character);
      }
      currentGoal = null;
      goalInProgress = false;
      path = null;
    }

    private IEnumerator EndGoal(Interactable goal) {
      yield return new WaitForSeconds(10);
      if (GetComponent<Agent>().currentHeldItem != null) {
        goal.PrimaryInteractionOff(character);
      }
    }

    private float GetDistance(Interactable currGoal){
      return Vector3.Distance(currGoal.transform.position, transform.position);
    }

    private void CalculatePath(Interactable currentGoal){
      NavMeshHit navHit;
      path = new NavMeshPath();
      NavMesh.SamplePosition(currentGoal.transform.position, out navHit, maxInteractionDistance, -1);
      navMeshAgent.CalculatePath(navHit.position, path);
      navMeshAgent.SetPath(path);
    }    
}
