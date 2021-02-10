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

    // [PunRPC]
    // private void movePlayer(Vector3 position) {
    //     CharacterController playerController = ((CharacterController)gameObject.GetComponent(typeof(CharacterController)));
    //     playerController.Move(new Vector3(0,10,0));
    //     //transform.position = position;
    // }

    // public void OnRobberCapture(GameObject robber) {
    //     Debug.Log("robber caught");
    //     PhotonView photonView = PhotonView.Get(this);
    //     photonView.RPC("movePlayer", RpcTarget.All, new Vector3(0,10,0));
    // }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        Debug.Log("colliding");
        if (hit.gameObject.tag == "seeker"){
            gameController.OnRobberCapture(gameObject);
        }
    }
}
