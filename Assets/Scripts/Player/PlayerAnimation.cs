using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerAnimation : MonoBehaviourPun
{

    private Animator animator;
    public CharacterController playerController;
    public string currAnimation;

    void Awake() {
        // If the player is not me (ie not some other player on the network)
        // then destory this script
        if (!photonView.IsMine) {
            Destroy(this);
        }

        // Dont destory a player on scene change
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // animator.SetFloat("Velocity", playerController.velocity.magnitude);

        if (Input.GetKeyDown(KeyCode.Z)) {
            // Debug.Log("anim1");
            animator.SetTrigger("Dancing");
            if (currAnimation != "Walking"){
              animator.ResetTrigger(currAnimation);
            } else {
              animator.SetBool("Walking",false);
            }
            currAnimation = "Dancing";
        }
        if (Input.GetKeyDown(KeyCode.X)) {
            // Debug.Log("anim2");
            animator.SetTrigger("Talking");
            if (currAnimation != "Walking"){
              animator.ResetTrigger(currAnimation);
            } else {
              animator.SetBool("Walking",false);
            }
            currAnimation = "Talking";
            // animator.ResetTrigger("Talking");
        }
        if (Input.GetKeyDown(KeyCode.W)) {
            animator.SetBool("Walking",true);
            if (currAnimation != "Walking"){
              animator.ResetTrigger(currAnimation);
            } else {
              animator.SetBool("Walking",false);
            }
            currAnimation = "Walking";
            // animator.SetBool("Walking",false);
        }
    }
}
