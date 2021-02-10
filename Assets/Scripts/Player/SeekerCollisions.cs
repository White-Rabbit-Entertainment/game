using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;


public class SeekerCollisions : MonoBehaviour {

    
    private GameManager manager;
    
    void Start() {
        manager = new GameManager();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        
        if (hit.gameObject.tag == "seeker"){
            manager.OnRobberCapture(gameObject);
        }
    }
}
