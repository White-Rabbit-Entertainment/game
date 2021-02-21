﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
//using UnityEngine.JSONSerializeModule;

public class GameManager : MonoBehaviourPun {

    // Instance
    public GameObject stealables;
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

    public void OnRobberCapture(GameObject robber) {
      PhotonView view = robber.GetComponent<PhotonView>();
      GameObject jail = GameObject.Find("/Jail/JailSpawn");
      NetworkManager.instance.SetPlayerProperty("Captured", true, view.Owner);
      view.RPC("MovePlayer", view.Owner, jail.transform.position);
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
          NetworkManager.instance.SetRoomProperty("NumberOfTargetItems", 2);
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
      //string jsonTaskString = JsonListHelper.ToJson(tasks);
      //Debug.Log(jsonTaskString);
      // StealingTask[] newTasks = JsonListHelper.FromJson<StealingTask>(jsonTaskString);
      StartRoundTimer();
    }

    public void HandleGameOver() {
      int secondsLeft = (int)NetworkManager.instance.GetRoundTimeRemaining();
      int itemsStolen = NetworkManager.instance.GetRoomProperty<int>("ItemsStolen");
      int numberOfTargetItems = NetworkManager.instance.GetRoomProperty<int>("NumberOfTargetItems");

      if (PhotonNetwork.CurrentRoom != null && SceneManager.GetActiveScene().name == "GameScene") {
        if (secondsLeft <= 0) {
          NetworkManager.instance.SetRoomProperty("WinningTeam", "Seeker");
        }

        if (NetworkManager.instance.AllRobbersCaught()) {
          NetworkManager.instance.SetRoomProperty("WinningTeam", "Seeker");
        }

        if (itemsStolen >= numberOfTargetItems) {
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

    Task[] GetTasks() {
      Task[] tasks = FindObjectsOfType<Task>();
      return tasks;
    }

    void Update() {
      HandleGameOver();
      Debug.Log(GetTasks());
    }
}
