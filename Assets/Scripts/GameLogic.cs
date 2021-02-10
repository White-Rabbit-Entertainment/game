using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameLogic : MonoBehaviour {

    // Instance
    public GameObject playerObject;
    public GameObject playerPrefab;
    public static GameLogic instance;
    public Transform jail;

    void Awake() {
      // If there is already a network manager instance then stop
      if (instance != null && instance != this) {
        gameObject.SetActive(false);
      }
      else { 
        // Otherwise set the instance to this class 
        instance = this;

        // When we change scenes (eg to game scene) dont desotry this instance
        DontDestroyOnLoad(gameObject);
      }
    }

    [PunRPC]
    private void movePlayer(GameObject player, Vector3 position) {
        player.transform.position = position;
    }

    public void OnRobberCapture(GameObject robber) {
        Debug.Log("robber caught");
        PhotonView photonView = PhotonView.Get(robber);
        photonView.RPC("movePlayer", RpcTarget.All, robber, new Vector3(0,10,0));
    }

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }
}
