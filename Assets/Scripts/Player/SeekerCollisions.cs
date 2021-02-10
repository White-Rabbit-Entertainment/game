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

    [PunRPC]
    private void movePlayer(Vector3 position) {
        CharacterController playerController = ((CharacterController)gameObject.GetComponent(typeof(CharacterController)));
        playerController.Move(new Vector3(0,10,0));
        //transform.position = position;
        //((PlayerMovement)gameObject.GetComponent(typeof(PlayerMovement))).enabled = true;
    }

    public void OnRobberCapture(GameObject robber) {
        Debug.Log("robber caught");
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("movePlayer", RpcTarget.All, new Vector3(0,10,0));
    }

    private void OnCollisionEnter(Collision collision) {
        Debug.Log("colliding");
        if (collision.gameObject.tag == "seeker"){
            //((PlayerMovement)gameObject.GetComponent(typeof(PlayerMovement))).enabled = false;
            
            OnRobberCapture(gameObject);            
        }
    }
}
