using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NpcMove : MonoBehaviour
{
    [SerializeField]
    Transform _destination;
    NavMeshAgent _navMeshAgent;
    public float wanderRadius = 30f;
    public int walk = 1;
    public int idle = 0;
    public int current_animation_state;
    public int frames = 0;
    // List<string> animationConditions = new List<string>();
    // // public var animation_conditions = new List<string>();
    // animation_conditions.Add("walk");
    // animation_conditions.Add("idle");
    // animation_conditions.Add("run");
    public float speed = 30f;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        _navMeshAgent = this.GetComponent<NavMeshAgent>();
        animator = this.GetComponent<Animator>();
        current_animation_state = idle;
    }

    void Update()
    {
      frames++;
      if(frames % 30 == 0)
      {
        int anim_cond = get_animation_condition();
        //animator.ResetTrigger(current)
        //animator.SetTrigger(anim_cond)
        if(anim_cond != current_animation_state){
          current_animation_state = anim_cond;
          animator.SetInteger("condition",current_animation_state);
        }
      }
      SetDestination(current_animation_state);
    }

    // Update is called once per frame
    private void SetDestination(int anim_cond)
    {
        if(_destination != null){
          Vector3 targetVector;
          if(anim_cond == walk){
            System.Console.WriteLine("walking...");
            targetVector = RandomWanderPoint();
          }
          // Vector3 targetVector = _destination.transform.position;
          else{
            targetVector = transform.position;
          };
          _navMeshAgent.speed = speed;
          _navMeshAgent.SetDestination(targetVector);
        }
    }

    public int get_animation_condition(){
      // return animation_conditions.PickRandom();
      System.Random r = new System.Random();
      int randNum = r.Next(0,2);
      return randNum;
    }

    public Vector3 RandomWanderPoint()
    {
      Vector3 randomPoint = (Random.insideUnitSphere * wanderRadius) + transform.position;
      NavMeshHit navHit;
      NavMesh.SamplePosition(randomPoint,out navHit,wanderRadius,-1);
      return new Vector3(navHit.position.x,transform.position.y,navHit.position.z);
    }
}
