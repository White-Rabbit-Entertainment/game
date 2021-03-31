using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;

public class LobbyRoomUI : MonoBehaviourPunCallbacks {
    public Text roomName;

    public GameObject playerList;
    public GameObject readyPlayerItemPrefab;
    public GameObject unreadyPlayerItemPrefab;

    public Task tutorialTask;

    bool initialized = false; 

    void Start() {
      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = true;
      NetworkManager.instance.ResetRoom();
      roomName.text = $"Room Name: {PhotonNetwork.CurrentRoom.Name}";
    }

    void Update() {
      if (!initialized && NetworkManager.instance.IsRoomReset()) {
        initialized = true;
      }
      if (initialized) {
        SetText();

        if (tutorialTask.isCompleted) {
          NetworkManager.instance.SetLocalPlayerProperty("Ready", true);
        }

        if (NetworkManager.instance.AllPlayersReady()) {
          NetworkManager.instance.SetupGame();
          if (NetworkManager.instance.RoomPropertyIs<bool>("GameReady", true)) {
            NetworkManager.instance.SetRoomProperty("GameStarted", true);
            NetworkManager.instance.ChangeScene("GameScene");
            Destroy(this);
          }
        }
      }
    }

    void SetText() {
      playerList.DestroyChildren();
      foreach (Player player in NetworkManager.instance.GetPlayers()) {
        GameObject playerItemPrefab; 
        if (NetworkManager.instance.PlayerPropertyIs("Ready", true, player)) {
          playerItemPrefab = readyPlayerItemPrefab;
        } else {
          playerItemPrefab = unreadyPlayerItemPrefab;
        }
        GameObject item = Instantiate(playerItemPrefab, transform);
        item.GetComponentInChildren<Text>().text = player.NickName;
        item.transform.SetParent(playerList.transform);
      }
    }

    void ToggleReady() {
      if (NetworkManager.instance.LocalPlayerPropertyIs<bool>("Ready", true)) {
        NetworkManager.instance.SetLocalPlayerProperty("Ready", false);
      } else {
        NetworkManager.instance.SetLocalPlayerProperty("Ready", true);
      }
    }
}
