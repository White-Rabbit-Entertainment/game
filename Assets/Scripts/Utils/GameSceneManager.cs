using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameSceneManager : MonoBehaviour {
    
    private bool enabled = true;
    private bool started = false;
    private bool initialized = false;

    public LoadingScreen loadingScreen;
    public PhotonView votingManager;

    // Start is called before the first frame update
    public void Init() {
        enabled = true;
        NetworkManager.instance.SetLocalPlayerProperty("GameSceneInitalized", true);
        NetworkManager.instance.GetMe().Unfreeze();
    }

    void Reset() {
        enabled = false;
        started = false;
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
        Timer.RoundTimer.Start(600);
      }
    }

    // Update is called once per frame
    void Update() {
        if (enabled) {
            if (!initialized) {
                // We have all the playable characters in the scene
                if (NetworkManager.instance.RoomPropertyIs<bool>("TasksSet", true)) {
                    Init();
                }
            }
            if (!started && NetworkManager.instance.CheckAllPlayers<bool>("GameSceneInitalized", true)) {
                loadingScreen.EnableButton();
                if (PhotonNetwork.IsMasterClient) {
                  StartRoundTimer();
                }
                started = true;
            }
           
        }
    }
}
