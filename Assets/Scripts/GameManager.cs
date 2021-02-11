using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class GameManager : MonoBehaviour {

    // Instance
    public GameObject playerPrefab;
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
      Debug.Log("Item Captured");
      NetworkManager.instance.IncrementRoomProperty("ItemsStolen");
      Destroy(item);
    }

    public void StartRoundTimer() {
      if (PhotonNetwork.LocalPlayer.IsMasterClient) {
        NetworkManager.instance.StartRoundTimer(600);
      }
    }

    public double TimeRemaining() {
      return NetworkManager.instance.GetRoundTimeRemaining();
    }

    // Start is called before the first frame update
    public void OnStartGame() {
      StartRoundTimer();
    }

    public void GameOver(bool robbersWin) {
      Debug.Log("GAME OVER");
      if (robbersWin) {
        Debug.Log("Robbers Win");
      } else {
        Debug.Log("Seekers Win");
      }
    }

    // Update is called once per frame
    void Update() {
      int secondsLeft = (int)NetworkManager.instance.GetRoundTimeRemaining();
      int itemsStolen = NetworkManager.instance.GetRoomProperty<int>("ItemsStolen");
      Debug.Log(itemsStolen.ToString());
      if (secondsLeft <= 0) {
        GameOver(false);
      }

      if (NetworkManager.instance.AllRobbersCaught()) {
        GameOver(false);
      }

      if (NetworkManager.instance.RoomPropertyIs<int>("ItemsStolen", 2)) {
        GameOver(true);
      }
    }
}
