using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour {

    // Instance
    public GameObject playerPrefab;
    public static GameManager instance;
    
    public enum Team
    {
      Robber,
      Seeker,
      None
    }
  
    void Awake() {
      if (instance != null && instance != this) {
        gameObject.SetActive(false);
      }
      else { 
        instance = this;
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
      GameObject jail = GameObject.Find("/Jail/JailSpawn");
      NetworkManager.instance.SetLocalPlayerProperty("Captured", true);
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

    public void HandleGameOver() {
      Team winner = Team.None;
      int secondsLeft = (int)NetworkManager.instance.GetRoundTimeRemaining();
      int itemsStolen = NetworkManager.instance.GetRoomProperty<int>("ItemsStolen");

      if (PhotonNetwork.CurrentRoom != null && SceneManager.GetActiveScene().name == "GameScene") {
        if (secondsLeft <= 0) {
          winner = Team.Seeker;
        }

        if (NetworkManager.instance.AllRobbersCaught()) {
          winner = Team.Seeker;
        }

        if (NetworkManager.instance.RoomPropertyIs<int>("ItemsStolen", 2)) {
          winner = Team.Robber;
        }
      }
      if (winner != Team.None) {
        Debug.Log("Game Over!");
        Debug.Log($"{winner}'s have won!");
      }
    }

    // Update is called once per frame
    void Update() {
      HandleGameOver();
    }
}
