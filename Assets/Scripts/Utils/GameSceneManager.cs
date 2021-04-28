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

public class GameSceneManager : MonoBehaviour {
    
    private bool started = false;
    private bool starting = false;
    private bool initialized = false;

    private GameObject loadingScreen;
    [SerializeField] private GameObject TraitorLoadingScreen;
    [SerializeField] private GameObject LoyalLoadingScreen;
    [SerializeField] private TimerCountdown timerCountdown;
    [SerializeField] private TaskCompletionUI taskCompletionUI;

    [SerializeField] private TimerManager timerManager;
    [SerializeField] private GameOverUI gameOverUI;

    public Color traitorColor = new Color(0.93f, 0.035f, 0.009f);
    public Color loyalColor = new Color(0.0f, 0.436f, 1.0f);

    
    void Start() {
        if (NetworkManager.instance.LocalPlayerPropertyIs<Team>("Team", Team.Loyal)) {
            loadingScreen = LoyalLoadingScreen;
        } else {
            loadingScreen = TraitorLoadingScreen;
        }
        loadingScreen.SetActive(true);
    }

    void Update() {
        if (!initialized) {
            // We have all the playable characters in the scene
            if (NetworkManager.instance.RoomPropertyIs<bool>("TasksSet", true)) {
                Init();
            }
        }
        if (initialized && !starting && NetworkManager.instance.CheckAllPlayers<bool>("GameSceneInitalized", true)) {
            loadingScreen.GetComponent<LoadingScreen>().EnableButton();
            if (PhotonNetwork.IsMasterClient) {
              StartroundTimer();
            }
            starting = true;
        }

        if (starting) {
            if (Timer.roundTimer.IsStarted()) {
                started = true;
            } 
        }

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
        // Debug.Log("Checking timer");
        // Debug.Log(Timer.roundTimer.IsStarted());
        // Debug.Log(Timer.roundTimer.TimeRemaining());
        if (Timer.roundTimer.IsComplete()) {
          Debug.Log("Time ran out");
          // If the timer has round out for us we can stop the local timer directly
          Timer.roundTimer.End();
          EndGame(Team.Traitor);
        } 
    }

    public void EndGame(Team winningTeam) {
      GetComponent<PhotonView>().RPC("EndGameRPC", RpcTarget.All, winningTeam);
    }


    [PunRPC]
    // Show the UI for the gameover
    public void EndGameRPC(Team winningTeam) {
        taskCompletionUI.UpdateBar();
        timerCountdown.Stop();
        gameOverUI.OnGameOver(winningTeam);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        NetworkManager.instance.GetMe().Freeze();
    }


    /// <summary> Check if the level has finished loading. It does this by
    /// checking if all items, players and AIs are spawned in. </summary> 
    public bool SceneLoaded() {
      return NetworkManager.instance.RoomPropertyIs<bool>("TasksSet", true) && NetworkManager.instance.AllCharactersSpawned();
    }
    
    public void StartroundTimer() {
      if (PhotonNetwork.LocalPlayer.IsMasterClient) {
        timerManager.StartTimer(Timer.roundTimer);
      }
    }
    
    public Vector3 RandomNavmeshLocation(float radius = 25f) {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radius;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1)) {
            finalPosition = hit.position;
        }
        Debug.Log($"{finalPosition.x},{finalPosition.y+3}, {finalPosition.z}");
        return new Vector3 (finalPosition.x,finalPosition.y+3,finalPosition.z);
    }

    public void GoToLobby() {
        NetworkManager.instance.ChangeScene("MenuScene");
    }
}
