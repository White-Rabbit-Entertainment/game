using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System;

public class GameSceneManager : MonoBehaviour {
    
    private bool started = false;
    private bool starting = false;
    private bool initialized = false;

    public LoadingScreen loadingScreen;
    //public SettlementUI settlementUI;
    //public GameSceneManager gameSceneManager;

    public GameObject traitorsWonUI;
    public GameObject loyalsWonUI;
    public GameObject traitorInfoUI;

    public Button nextButtonTraitor;
    public Button nextButtonLoyal;

    public Text traitorName;

    


    void Update() {
        if (!initialized) {
            // We have all the playable characters in the scene
            if (NetworkManager.instance.RoomPropertyIs<bool>("TasksSet", true)) {
                Init();
            }
        }
        if (initialized && !starting && NetworkManager.instance.CheckAllPlayers<bool>("GameSceneInitalized", true)) {
            loadingScreen.EnableButton();
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
        //EndGame(Team.Traitor);
      } 
    }

    public void EndGame(Team winningTeam) {
      GetComponent<PhotonView>().RPC("EndGameRPC", RpcTarget.All, winningTeam);
    }

   
    

    [PunRPC]
    public void EndGameRPC(Team winningTeam) {
        // NetworkManager.instance.ChangeScene("LobbyScene");

        //SettlementUI sl = new SettlementUI();

        //sl.OnGameOver(winningTeam);

        OnGameOver(winningTeam);
    }



    /// <summary> Check if the level has finished loading. It does this by
    /// checking if all items, players and AIs are spawned in. </summary> 
    // TODO Show some loading UI if the level isnt loaded yet.
    // TODO Check players are loaded in.
    // TODO Check AIs are loaded in.
    public bool SceneLoaded() {
      return NetworkManager.instance.RoomPropertyIs<bool>("TasksSet", true) && NetworkManager.instance.AllCharactersSpawned();
    }
    
    public void StartRoundTimer() {
      if (PhotonNetwork.LocalPlayer.IsMasterClient) {
        Timer.RoundTimer.Start(240);
      }
    }

    

    
    public void OnGameOver(Team team)
    {
        nextButtonTraitor.onClick.AddListener(GoToLobby);
        nextButtonLoyal.onClick.AddListener(GoToLobby);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        traitorName.text = NetworkManager.traitorNames.ToString();
        if (team == Team.Traitor)
        {
            traitorsWonUI.SetActive(true);
            traitorInfoUI.SetActive(true);
        }
        else
        {
            loyalsWonUI.SetActive(true);
            traitorInfoUI.SetActive(true);
        }
    }

    void GoToLobby()
    {
        NetworkManager.instance.ChangeScene("LobbyScene");
    }


    List<string> GetTraitorNames() {
        List<string> names = new List<string>();
        foreach (Player player in NetworkManager.instance.GetPlayers()) {
            if (NetworkManager.instance.PlayerPropertyIs("Traitor", true, player)) {
                names.Add(player.NickName);
            }
        }

        return names;
    }
}
