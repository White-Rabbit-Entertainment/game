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
    public GameObject TraitorLoadingScreen;
    public GameObject LoyalLoadingScreen;
    public TimerCountdown timerCountdown;
    public TaskCompletionUI taskCompletionUI;

    public GameObject playersWonUI;
    public GameObject traitorInfoUI;
    public TextMeshProUGUI playerDescriptionText;
    public Button nextButton;
    public Text traitorName;

    
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
              StartRoundTimer();
            }
            starting = true;
        }

        if (starting) {
            if (Timer.RoundTimer.IsStarted()) {
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
      if (Timer.RoundTimer.IsComplete()) {
        Debug.Log("Time ran out");
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
        
        nextButton.onClick.AddListener(GoToLobby);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        NetworkManager.instance.GetMe().Freeze();

        traitorName.text = string.Join(", ", NetworkManager.traitorNames);
        playersWonUI.SetActive(true);
        if (winningTeam == Team.Traitor) {
            playerDescriptionText.text = "Traitors Won!";
        }
        else {
            playerDescriptionText.text = "Loyals Won!";
        }
    }


    /// <summary> Check if the level has finished loading. It does this by
    /// checking if all items, players and AIs are spawned in. </summary> 
    public bool SceneLoaded() {
      return NetworkManager.instance.RoomPropertyIs<bool>("TasksSet", true) && NetworkManager.instance.AllCharactersSpawned();
    }
    
    public void StartRoundTimer() {
      if (PhotonNetwork.LocalPlayer.IsMasterClient) {
        Timer.RoundTimer.Start(1000);
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

    void GoToLobby() {
        NetworkManager.instance.ChangeScene("MenuScene");
    }
}
