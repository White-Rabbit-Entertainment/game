using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{   

    private Animator animator;
    public CharacterController playerController;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Velocity", playerController.velocity.magnitude);

        if (Input.GetKeyDown(KeyCode.Z)) {
            Debug.Log("anim1");
            animator.SetTrigger("Dummy1");
            animator.ResetTrigger("Dummy2");
        }
        if (Input.GetKeyDown(KeyCode.X)) {
            Debug.Log("anim2");
            animator.SetTrigger("Dummy2");
            animator.ResetTrigger("Dummy1");

        }
    }
}
