using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerAnimation : MonoBehaviourPun {   

    private Animator animator;
    public CharacterController playerController;

    int crouchHash = Animator.StringToHash("Crouched");
    int proneHash = Animator.StringToHash("Prone");

    void Awake() {
        // If the player is not me (ie not some other player on the network)
        // then destory this script
        if (photonView != null && !photonView.IsMine) {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start() {
        animator = GetComponent<Animator>();
        animator.SetBool("Crouched", false);
        animator.SetBool("Prone", false);
    }

    // Update is called once per frame
    void Update() {      
        animator.SetFloat("Walking", GetComponent<Character>().Velocity().magnitude);
        if (Input.GetKeyDown(KeyCode.Z)) {
            animator.SetTrigger("Dancing");
        }
        if (Input.GetKeyDown(KeyCode.X)) {
            animator.SetTrigger("Talking");
        }
        if (Input.GetKeyDown(KeyCode.C)) {
            animator.SetBool(crouchHash, !animator.GetBool(crouchHash));
        }
        if (Input.GetKeyDown(KeyCode.V)) {
            animator.SetBool(proneHash, !animator.GetBool(proneHash));
        }
    }
}
