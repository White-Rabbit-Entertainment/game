using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;


public class SeekerCollisions : MonoBehaviour {

    
    private GameLogic gameController;
    
    void Start() {
        gameController = new GameLogic();
        if (gameController != null) {
            Debug.Log("Created gameController");
        } else {
            Debug.Log("Could not create gameController");
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        
        if (hit.gameObject.tag == "seeker"){
            gameController.OnRobberCapture(gameObject);
        }
    }
}
