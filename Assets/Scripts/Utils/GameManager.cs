using System;
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

    private List<Player> playersLeft;
  
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
        NetworkManager.instance.StartRoundTimer(10);
      }
    }

    public void StartTurnTimer() {
      if (PhotonNetwork.LocalPlayer.IsMasterClient) {
        NetworkManager.instance.StartTurnTimer(5);
      }
    }

    public void StartMealSwapTimer() {
      if (PhotonNetwork.LocalPlayer.IsMasterClient) {
        NetworkManager.instance.StartRoundTimer(NetworkManager.instance.GetPlayers().Count * 5 + 10);
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

          List<Role> roles = Enum.GetValues(typeof(Role)).Cast<Role>().ToList();
          roles.Remove(Role.Captain); // We dont want to assing anyone (expect the capatian) the capatian role

          // Shuffle players and roles to ensure random team and role are assigned
          roles.Shuffle();
          players.Shuffle();

          for (int i = 0; i < numberOfTraitors; i++) {
            NetworkManager.instance.SetPlayerProperty("Team", Team.NonCaptainLoyal, players[i]);
            NetworkManager.instance.SetPlayerProperty("Role", roles[i % roles.Count], players[i]);
          }

          NetworkManager.instance.SetPlayerProperty("Team", Team.Captain, players[numberOfTraitors]);
          NetworkManager.instance.SetPlayerProperty("Role", Role.Captain, players[numberOfTraitors]);

          for (int i = numberOfTraitors + 1; i < players.Count; i++) {
            NetworkManager.instance.SetPlayerProperty("Team", Team.NonCaptainLoyal, players[i]);
            NetworkManager.instance.SetPlayerProperty("Role", roles[(i - 1) % roles.Count], players[i]);
          }
          NetworkManager.instance.SetRoomProperty("GameReady", true);

        }
      }
    }

    public void StartGame() {
      NetworkManager.instance.SetLocalPlayerProperty("InGameScene", false);
      NetworkManager.instance.ChangeScene("GameScene");
      StartRoundTimer();
    }

     public void StartMealSwap() {
        // Set the players to not in game scene for player spawner 
        NetworkManager.instance.SetLocalPlayerProperty("InGameScene", false);

        if (PhotonNetwork.LocalPlayer.IsMasterClient) {
          NetworkManager.instance.SetRoomProperty("CurrentPlayerGuessed", false);    
          NetworkManager.instance.SetRoomProperty("CurrentScene", "MealScene");
          
          playersLeft = NetworkManager.instance.GetPlayers();
          NetworkManager.instance.SetRoomProperty("CurrentPlayerId", playersLeft[0].UserId);
        }
        // NetworkManager.instance.SetRoomProperty("PlayersLeftToSwap", playerStrings);
        NetworkManager.instance.ChangeScene("MealScene");
        StartMealSwapTimer();
        StartTurnTimer();
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

     public void HandleSceneSwitch(){
        int secondsLeft = (int)NetworkManager.instance.GetRoundTimeRemaining();
        if (secondsLeft < 0) {
          // Store scene in the master client
          if (PhotonNetwork.CurrentRoom != null && SceneManager.GetActiveScene().name == "GameScene") {
            StartMealSwap();
          }
        }
        if (SceneManager.GetActiveScene().name == "MealScene" && NetworkManager.instance.RoomPropertyIs<string>("CurrentScene", "GameScene")) {
          StartGame();
        }
    }

     public void CurrentPlayerSwitching(){
        // if (playersLeft.Count > 1) Debug.Log("BIGGER");
        // List<string> playersLeft = NetworkManager.instance.GetRoomProperty<List<string>>("PlayersLeftToSwap");
        if (PhotonNetwork.LocalPlayer.IsMasterClient) {
          
          // How long left does the current player have in their turn
          int secondsLeft = (int)NetworkManager.instance.GetTurnTimeRemaining();
         
          // If their round has now ended
          if (secondsLeft < 0 || (NetworkManager.instance.GetRoomProperty<bool>("CurrentPlayerGuessed"))) {

            // Start a new round
            StartTurnTimer();

            // Pop the player who just had a round
            playersLeft.RemoveAt(0);
           
            // If there are no players left the meal scene is over
            if (playersLeft.Count == 0) {
              NetworkManager.instance.SetRoomProperty("CurrentScene", "GameScene");
            } else {
            // Otherwise Assign the correct player to that round
              NetworkManager.instance.SetRoomProperty("CurrentPlayerId", playersLeft[0]);
              NetworkManager.instance.SetRoomProperty("CurrentPlayerGuessed", false); 
            }
          } 
        }
    }

    /// <summary> Check if the level has finished loading. It does this by
    /// checking if all items, players and AIs are spawned in. </summary> 
    // TODO Show some loading UI if the level isnt loaded yet.
    // TODO Check players are loaded in.
    // TODO Check AIs are loaded in.
    public bool SceneLoaded() {
      return NetworkManager.instance.RoomPropertyIs<bool>("TasksSet", true) && NetworkManager.instance.AllCharactersSpawned();
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
      if (SceneLoaded()) {
        HandleSceneSwitch();
        if (PhotonNetwork.CurrentRoom != null && SceneManager.GetActiveScene().name == "MealScene") CurrentPlayerSwitching();
        // HandleGameOver();
      }
  }
}
