using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class AgentAnimation : MonoBehaviour
{
    public string current_animation_state;
    List<string> animationConditions = new List<string>() {
      "walk","idle"
    };
    Animator animator;
    NavMeshAgent navMeshAgent;
    private Vector3 previousPosition;
    public float curSpeed;
    // Start is called before the first frame update
    void Start()
    {
      animator = this.GetComponent<Animator>();
      navMeshAgent = this.GetComponent<NavMeshAgent>();
      previousPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
      Vector3 curMove = transform.position - previousPosition;
      curSpeed = curMove.magnitude / Time.deltaTime;
      previousPosition = transform.position;
      animator.SetFloat("Walking", curSpeed);
      // string anim_cond = get_animation_condition();



    }

    // public string get_animation_condition(){
    //   // return animation_conditions.PickRandom();
    //   System.Random r = new System.Random(System.Guid.NewGuid().GetHashCode());
    //   int randNum = r.Next(animationConditions.Count);
    //   return (string)animationConditions[randNum];
    // }
}
