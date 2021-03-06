﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
//using UnityEngine.JSONSerializeModule;

/// <summary><c>GameManager</c> is the brain of the game. It contains most of
/// the important logic such as GameOver logic. The game manager is a singleton
/// (meaning one static instance of it exists). It is created in the first
/// (multiplayer menu) scene. It can be referenced at any time with <code>
/// GameManager.instance </code></summary> 
public class GameManager : MonoBehaviourPun {

    // Instance
    public GameObject stealables;
    public GameObject loyalPrefab;
    public GameObject traitorPrefab;

    // Current instance of the GameManager singleton
    public static GameManager instance;
  
    void Awake() {
      /// This is what makes this a singleton. This means we can do <code>
      /// GameManager.instance </code> in other files to access the gamemanger
      /// instance.
      // TODO Move this into a singleton class so we can just inherit this.
      if (instance != null && instance != this) {
        gameObject.SetActive(false);
      }
      else { 
        instance = this;
        DontDestroyOnLoad(gameObject);
      }
    }

    public void StartRoundTimer() {
      if (PhotonNetwork.LocalPlayer.IsMasterClient) {
        NetworkManager.instance.StartRoundTimer(600);
      }
    }

    public double TimeRemaining() {
      return NetworkManager.instance.GetRoundTimeRemaining();
    }

    /// <summary> Before a game is able to start various things need to be
    /// setup. Such as which team each player is on. </summary>
    public void SetupGame() {
      if (NetworkManager.instance.RoomPropertyIs<bool>("GameStarted", false)) {
        if (PhotonNetwork.LocalPlayer.IsMasterClient) {
          NetworkManager.instance.SetRoomProperty("TasksSet", false);
          NetworkManager.instance.SetRoomProperty("WinningTeam", "None");
          NetworkManager.instance.SetRoomProperty("NumberOfStealingTasks", 2);
          NetworkManager.instance.SetRoomProperty("NumberOfNonStealingTasks", 2);
          
          List<Player> players = NetworkManager.instance.GetPlayers();
          int numberOfTraitors = NetworkManager.instance.GetRoomProperty<int>("NumberOfTraitors", (int)(players.Count/2));

          players.Shuffle();
          for (int i = 0; i < numberOfTraitors; i++) {
            NetworkManager.instance.SetPlayerProperty("Team", Team.Traitor, players[i]);
          }

          NetworkManager.instance.SetPlayerProperty("Team", Team.Captain, players[numberOfTraitors]);

          for (int i = numberOfTraitors + 1; i < players.Count; i++) {
            NetworkManager.instance.SetPlayerProperty("Team", Team.NonCaptainLoyal, players[i]);
          }
          NetworkManager.instance.SetRoomProperty("GameReady", true);
        }
      }
    }

    public void StartGame() {
      NetworkManager.instance.ChangeScene("GameScene");
      StartRoundTimer();
    }

    /// <summary> This function handles the game over logic. It does 2 things:
    ///   <list> 
    ///     <item> Check if it thinks any team has won. If so it sets that team
    ///     as the winner on the room so all clients know. </item>
    ///     <item> Cheks if a team has been set as the winner (by local or any
    ///     other client), if so its end the game and returns the client to the
    ///     lobby. 
    ///   <list> 
    /// </summary>
    public void HandleGameOver() {
      int secondsLeft = (int)NetworkManager.instance.GetRoundTimeRemaining();

      if (PhotonNetwork.CurrentRoom != null && SceneManager.GetActiveScene().name == "GameScene") {
        if (secondsLeft <= 0) {
          NetworkManager.instance.SetRoomProperty("WinningTeam", Team.Loyal);
        }

        if (NetworkManager.instance.NoLoyalsRemaining()) {
          NetworkManager.instance.SetRoomProperty("WinningTeam", Team.Traitor);
        }

        if (NetworkManager.instance.CaptainIsDead()) {
          NetworkManager.instance.SetRoomProperty("WinningTeam", Team.Traitor);
        }

        if (AllTasksCompleted()) {
          NetworkManager.instance.SetRoomProperty("WinningTeam", Team.Loyal);
        }
        
        if (!NetworkManager.instance.RoomPropertyIs<Team>("WinningTeam", Team.None)) {
          Team winner = NetworkManager.instance.GetRoomProperty<Team>("WinningTeam");
          string winnerString;
          if (winner == Team.Traitor) winnerString = "Traitor";
          else winnerString = "Loyal";
          Debug.Log("Game Over!");
          Debug.Log($"{winnerString}'s have won!");
          NetworkManager.instance.ResetRoom();
          NetworkManager.instance.ChangeScene("LobbyScene");
        }
      }  
    }

    /// <summary> Check if the level has finished loading. It does this by
    /// checking if all items, players and AIs are spawned in. </summary> 
    // TODO Show some loading UI if the level isnt loaded yet.
    // TODO Check players are loaded in.
    // TODO Check AIs are loaded in.
    public bool LevelLoaded() {
      return NetworkManager.instance.RoomPropertyIs<bool>("TasksSet", true);
    }

    /// <summary> Return all the tasks in the scene. I.e. all the tasks the
    /// Loyals have to do to win the game. </summary> 
    public Task[] GetTasks() {
      Task[] tasks = FindObjectsOfType<Task>();
      return tasks;
    }

    /// <summary> Return if all Loyal tasks have been completed. </summary> 
    public bool AllTasksCompleted() {
      foreach(Task task in GetTasks()) {
        if (!task.isCompleted) {
          return false;
        }
      }
      return true;
    }

    void Update() {
      if (LevelLoaded()) {
        HandleGameOver();
      }
    }
}
