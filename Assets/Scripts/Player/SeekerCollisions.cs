using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;


public class SeekerCollisions : MonoBehaviour {
    private void OnControllerColliderHit(ControllerColliderHit hit) {
        
        if (hit.gameObject.tag == "seeker"){
            GameManager.instance.OnRobberCapture(gameObject);
        }
    }
}
