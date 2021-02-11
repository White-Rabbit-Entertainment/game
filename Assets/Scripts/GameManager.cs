using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class GameManager : MonoBehaviour {

    // Instance
    public GameObject playerPrefab;
    public NetworkManager networkManager;
    public static GameManager instance;
    public GameObject jail;
  
    void Awake() {
      // If there is already a network manager instance then stop
      if (instance != null && instance != this) {
        gameObject.SetActive(false);
      }
      else { 
        // Otherwise set the instance to this class
        instance = this;
        networkManager = new NetworkManager();
        // When we change scenes (eg to game scene) dont destroy this instance
        DontDestroyOnLoad(gameObject);
      }
    }

    private void MovePlayer(GameObject player, Vector3 position) {
      CharacterController characterController = player.GetComponent<CharacterController>();
	    characterController.enabled = false;
	    player.transform.position = position;
	    characterController.enabled = true;
    }

    public void OnRobberCapture(GameObject robber) {
      jail = GameObject.Find("/Jail/JailSpawn");
      NetworkManager.instance.SetLocalPlayerProperty("Captured", true);
      Debug.Log(jail.transform.position);
      MovePlayer(robber, jail.transform.position);
    }

    public void OnItemInSafeZone(GameObject item) {
      networkManager = new NetworkManager();
      Debug.Log("Item Captured");
      networkManager.IncrementRoomProperty("ItemsStolen");
      Destroy(item);
    }

    public void StartRoundTimer() {
      networkManager = new NetworkManager();
      if (PhotonNetwork.LocalPlayer.IsMasterClient) {
        networkManager.StartRoundTimer(500);
      }
    }

    public double TimeRemaining() {
      networkManager = new NetworkManager();
      return networkManager.GetRoundTimeRemaining();
    }

    // Start is called before the first frame update
    public void OnStartGame() {
      StartRoundTimer();
    }

    // Update is called once per frame
    void Update() {
        
    }
}
