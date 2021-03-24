﻿using System;
using System.Collections;
using System.Linq;
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

    public double TimeRemaining() {
      return Timer.RoundTimer.TimeRemaining();
    }

    /// <summary> Before a game is able to start various things need to be
    /// setup. Such as which team each player is on. </summary>
    public void SetupGame() {
      if (NetworkManager.instance.RoomPropertyIs<bool>("GameStarted", false)) {
        if (PhotonNetwork.LocalPlayer.IsMasterClient) {
          NetworkManager.instance.SetRoomProperty("TasksSet", false);
          NetworkManager.instance.SetRoomProperty("WinningTeam", "None");
          NetworkManager.instance.SetRoomProperty("NumberOfTasks", 10);
          
          List<Player> players = NetworkManager.instance.GetPlayers();
          int numberOfTraitors = NetworkManager.instance.GetRoomProperty<int>("NumberOfTraitors", (int)(players.Count/2));

          List<Role> roles = Enum.GetValues(typeof(Role)).Cast<Role>().ToList();

          // Shuffle players and roles to ensure random team and role are assigned
          roles.Shuffle();
          players.Shuffle();

          for (int i = 0; i < numberOfTraitors; i++) {
            NetworkManager.instance.SetPlayerProperty("Team", Team.Traitor, players[i]);
            NetworkManager.instance.SetPlayerProperty("Role", roles[i % roles.Count], players[i]);
          }

          for (int i = numberOfTraitors; i < players.Count; i++) {
            NetworkManager.instance.SetPlayerProperty("Team", Team.Loyal, players[i]);
            NetworkManager.instance.SetPlayerProperty("Role", roles[i % roles.Count], players[i]);
          }
          NetworkManager.instance.SetRoomProperty("GameReady", true);

        }
      }
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
      if (PhotonNetwork.CurrentRoom != null && SceneManager.GetActiveScene().name == "GameScene") {
        if (Timer.RoundTimer.IsComplete()) {
          Debug.Log("Time has run out");
          NetworkManager.instance.SetRoomProperty("WinningTeam", Team.Traitor);
        }

        Team winner = NetworkManager.instance.GetRoomProperty<Team>("WinningTeam");
        if (winner != Team.None) {
          Debug.Log($"Game Over!\n{winner.ToString()}'s have won!");
          NetworkManager.instance.ResetRoom();
          NetworkManager.instance.ChangeScene("LobbyScene");
        }
      }  
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
      HandleGameOver();
    }
}
