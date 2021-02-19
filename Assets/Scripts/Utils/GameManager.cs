﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviourPun {

    // Instance
    public GameObject robberPrefab;
    public GameObject seekerPrefab;
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

    // [PunRPC]
    // private void TransferOwnership(PhotonMessageInfo info) {
    //   Debug.Log("transferring ownership");
    //   info.photonView.TransferOwnership(info.Sender);
    // }

    // private void MovePlayer(GameObject player, Vector3 position) {
    //   CharacterController characterController = player.GetComponent<CharacterController>();
	  //   characterController.enabled = false;
	  //   player.transform.position = position;
	  //   characterController.enabled = true;
    // }

    public void OnRobberCapture(GameObject robber) {
      Debug.Log("Starting OnRobberCapture");
      PhotonView view = robber.GetComponent<PhotonView>();
      GameObject jail = GameObject.Find("/Jail/JailSpawn");
      view.RPC("MovePlayer", RpcTarget.All, jail.transform.position);
      Debug.Log("Finished OnRobberCapture");
      // Player owner = view.Owner;
      // Debug.Log(owner);
      // view.RPC("TransferOwnership", RpcTarget.All);
      // GameObject jail = GameObject.Find("/Jail/JailSpawn");
      // NetworkManager.instance.SetLocalPlayerProperty("Captured", true);
      // MovePlayer(robber, jail.transform.position);
      //photonView.RPC("TransferOwnership", RpcTarget.All);
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

    public void SetupGame() {
      if (NetworkManager.instance.RoomPropertyIs<bool>("GameStarted", false)) {
        if (PhotonNetwork.LocalPlayer.IsMasterClient) {
          NetworkManager.instance.SetRoomProperty("WinningTeam", "None");
          List<Player> players = NetworkManager.instance.GetPlayers();
          int numberOfRobbers = NetworkManager.instance.GetRoomProperty<int>("NumberOfRobbers", (int)(players.Count/2));
          players.Shuffle();
          for (int i = 0; i < numberOfRobbers; i++) {
            NetworkManager.instance.SetPlayerProperty("Team", "Robber", players[i]);
          }

          for (int i = numberOfRobbers; i < players.Count; i++) {
            NetworkManager.instance.SetPlayerProperty("Team", "Seeker", players[i]);
          }
          NetworkManager.instance.SetRoomProperty("GameReady", true);
        }
      }
    }

    public void StartGame() {
      // Player spawning is now handled by the player spawner in GameScene
      NetworkManager.instance.ChangeScene("GameScene");
      StartRoundTimer();
    }

    public void HandleGameOver() {
      int secondsLeft = (int)NetworkManager.instance.GetRoundTimeRemaining();
      int itemsStolen = NetworkManager.instance.GetRoomProperty<int>("ItemsStolen");

      if (PhotonNetwork.CurrentRoom != null && SceneManager.GetActiveScene().name == "GameScene") {
        if (secondsLeft <= 0) {
          NetworkManager.instance.SetRoomProperty("WinningTeam", "Seeker");
        }

        if (NetworkManager.instance.AllRobbersCaught()) {
          NetworkManager.instance.SetRoomProperty("WinningTeam", "Seeker");
        }

        if (NetworkManager.instance.RoomPropertyIs<int>("ItemsStolen", 2)) {
          NetworkManager.instance.SetRoomProperty("WinningTeam", "Robber");
        }
        if (!NetworkManager.instance.RoomPropertyIs<string>("WinningTeam", "None")) {
          string winner = NetworkManager.instance.GetRoomProperty<string>("WinningTeam");
          Debug.Log("Game Over!");
          Debug.Log($"{winner}'s have won!");
          NetworkManager.instance.ResetRoom();
          NetworkManager.instance.ChangeScene("LobbyScene");
        }
      }
      
    }

    void Update() {
      HandleGameOver();
    }
}
