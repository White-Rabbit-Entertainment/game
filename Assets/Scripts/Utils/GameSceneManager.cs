using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System;
using TMPro;

// The main manager class for the game scene. This handles the central logic
// for the gamescene.
public class GameSceneManager : MonoBehaviour {
  
    // Synchronisation values
    private bool started = false;
    private bool starting = false;
    private bool initialized = false;

    // Loading screens
    private GameObject loadingScreen; // Actual loading screen for this player
    [SerializeField] private GameObject TraitorLoadingScreen;
    [SerializeField] private GameObject LoyalLoadingScreen;

    // UI
    [SerializeField] private TimerCountdown timerCountdown;
    [SerializeField] private TaskCompletionUI taskCompletionUI;
    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private DeathUI deathUI;

    // Managers
    [SerializeField] private TimerManager timerManager;

    // The places an item/player can be spawned
    [SerializeField] private GameObject spawnPointsGO;

    public Color traitorColor = new Color(0.93f, 0.035f, 0.009f);
    public Color loyalColor = new Color(0.0f, 0.436f, 1.0f);

    
    void Start() {
        // Show the loading screen. Selects loading screen from player team and
        // enables.
        if (NetworkManager.instance.LocalPlayerPropertyIs<Team>("Team", Team.Loyal)) {
            loadingScreen = LoyalLoadingScreen;
        } else {
            loadingScreen = TraitorLoadingScreen;
        }
        loadingScreen.SetActive(true);
    }

    void Update() {
        // initialize the game once all the tasks are set
        if (!initialized) {
            // We have all the playable characters in the scene
            if (NetworkManager.instance.RoomPropertyIs<bool>("TasksSet", true)) {
                Init();
            }
        }

        // Once initalized and all the players have inited then
        if (initialized && !starting && NetworkManager.instance.CheckAllPlayers<bool>("GameSceneInitalized", true)) {
            // Let the players close the loading screen
            loadingScreen.GetComponent<LoadingScreen>().EnableButton();
            // Start the round timer
            if (PhotonNetwork.IsMasterClient) {
              StartroundTimer();
            }
            starting = true;
        }

        // Once the timer is started then set the game as started
        if (starting) {
            if (Timer.roundTimer.IsStarted()) {
                started = true;
            } 
        }

        // Once its started then we can start checking if the game has finished
        if (started) {
            CheckTimer();
        }
    }

    // Start is called before the first frame update
    public void Init() {
        initialized = true;
        NetworkManager.instance.SetLocalPlayerProperty("GameSceneInitalized", true);
        NetworkManager.instance.GetMe().Unfreeze();
    }

    /// <summary> This function handles the game over logic. It does 2 things:
    ///   <list> 
    ///     <item> Check if it thinks any team has won. If so it sets that team
    ///     as the winner on the room so all clients know. </item>
    ///     <item> Cheks if a team has been set as the winner (by local or any
    ///     other client), if so its end the game and returns the client to the
    ///     lobby. 
    ///   <list>     
    public void CheckTimer() {
        if (PhotonNetwork.IsMasterClient && Timer.roundTimer.IsComplete()) {
          // If the timer has round out for us we can stop the local timer directly
          Timer.roundTimer.End();
          EndGame(Team.Traitor);
        } 
    }

    // Ends the game for all players
    public void EndGame(Team winningTeam) {
      GetComponent<PhotonView>().RPC("EndGameRPC", RpcTarget.All, winningTeam);
    }


    [PunRPC]
    // Show the UI for the gameover
    public void EndGameRPC(Team winningTeam) {
        // Stop timer
        Timer.roundTimer.End();

        // Show endgame UI
        taskCompletionUI.UpdateBar();
        timerCountdown.Stop();
        deathUI.gameObject.SetActive(false);
        gameOverUI.OnGameOver(winningTeam);
       
        // Unlock mouse
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Stop the player from being able to move (so mouse movement only
        // affects the mouse in the UI)
        NetworkManager.instance.GetMe().Freeze();

        // After a game has ended people can join the room again
        PhotonNetwork.CurrentRoom.IsVisible = true;
    }


    /// <summary> Check if the level has finished loading. It does this by
    /// checking if all items, players and AIs are spawned in. </summary> 
    public bool SceneLoaded() {
      return NetworkManager.instance.RoomPropertyIs<bool>("TasksSet", true) && NetworkManager.instance.AllCharactersSpawned();
    }
   
    // Master client starts the game round timer
    public void StartroundTimer() {
      if (PhotonNetwork.LocalPlayer.IsMasterClient) {
        timerManager.StartTimer(Timer.roundTimer);
      }
    }
   
    // Find a point on the map (Note: Implementation has been updated to use
    // spawn point as these allows us to choose exactly where a player/item can
    // spawn).
    public Vector3 RandomNavmeshLocation(float radius = 50f) {
        List<SpawnPoint> spawnPoints = new List<SpawnPoint>(spawnPointsGO.GetComponentsInChildren<SpawnPoint>());
        int randomIndex = UnityEngine.Random.Range(0, spawnPoints.Count); 
        return spawnPoints[randomIndex].transform.position;
    }

    // Change scene back to the lobby scene (MenuManager handle putting player
    // on correct page of menu).
    public void GoToLobby() {
        NetworkManager.instance.ChangeScene("MenuScene");
    }
}
