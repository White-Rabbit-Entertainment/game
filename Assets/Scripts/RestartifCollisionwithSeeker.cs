using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class RestartifCollisionwithSeeker : MonoBehaviour
{
 private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "seeker")
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
