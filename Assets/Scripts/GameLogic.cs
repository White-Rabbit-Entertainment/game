using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameLogic : MonoBehaviour {

    // Instance
    public GameObject playerPrefab;
    public static GameLogic instance;
    private GameObject jail;
  
    void Awake() {
      // If there is already a network manager instance then stop
      if (instance != null && instance != this) {
        gameObject.SetActive(false);
      }
      else { 
        // Otherwise set the instance to this class 
        instance = this;

        // When we change scenes (eg to game scene) dont destroy this instance
        DontDestroyOnLoad(gameObject);
      }
    }

    private void movePlayer(GameObject player, Vector3 position) {
      CharacterController characterController = player.GetComponent<CharacterController>();
	    characterController.enabled = false;
	    player.transform.position = position;
	    characterController.enabled = true;
    }

    public void OnRobberCapture(GameObject robber) {
      jail = GameObject.Find("/Jail/JailSpawn");
      Debug.Log(jail.transform.position);
      movePlayer(robber, jail.transform.position);
    }

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }
}
