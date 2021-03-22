using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameSceneManager : MonoBehaviour {
    
    private bool enabled = true;
    private bool started = false;
    private bool initialized = false;

    public LoadingScreen loadingScreen;

    public MealSceneManager mealSceneManager;

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
    
    public void HandleSceneSwitch(){
        if (PhotonNetwork.IsMasterClient) {
          int secondsLeft = (int)NetworkManager.instance.GetTimeRemaining(Timer.RoundTimer);
          if (secondsLeft <= 0 && NetworkManager.instance.IsTimerStarted(Timer.RoundTimer)) {
              NetworkManager.instance.SetRoomProperty("CurrentScene", "MealScene");
              NetworkManager.instance.EndTimer(Timer.RoundTimer);
              GetComponent<PhotonView>().RPC("SwitchToMealScene", RpcTarget.All);
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
    
    public void StartRoundTimer() {
      if (PhotonNetwork.LocalPlayer.IsMasterClient) {
        NetworkManager.instance.StartTimer(30, Timer.RoundTimer);
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
            if (started) {
                if (PhotonNetwork.IsMasterClient) {
                    HandleSceneSwitch();
                }
            }
        }
    }

    [PunRPC]
    void SwitchToMealScene() {
        // Disable all interactables
        NetworkManager.instance.SetLocalPlayerProperty("GameSceneInitalized", false);
        mealSceneManager.Init();
        NetworkManager.instance.GetMe().Freeze();
        Reset();
    }
}
