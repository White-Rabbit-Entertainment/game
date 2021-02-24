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
    public CharacterController playerController;
    // Start is called before the first frame update
    void Start()
    {
      animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
      animator.SetFloat("Velocity", playerController.velocity.magnitude);
      string anim_cond = get_animation_condition();



    }

    public string get_animation_condition(){
      // return animation_conditions.PickRandom();
      System.Random r = new System.Random(System.Guid.NewGuid().GetHashCode());
      int randNum = r.Next(animationConditions.Count);
      return (string)animationConditions[randNum];
    }
}
