using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;

public class LobbyPage : MenuPage {
    public Text playerCounter;
    public Text roomName;

    public GameObject playerList;
    public Button toggleReadyButton;
    public GameObject readyPlayerItemPrefab;
    public GameObject unreadyPlayerItemPrefab;

    public JoinRoomPage joinRoomPage;
    public Button back;

    bool initialized;
    bool enteredRoom; 

    void Start() {
      Debug.Log("Start");
      Cursor.lockState = CursorLockMode.None;
      Cursor.visible = true;
      initialized = false;
      enteredRoom = false;
      back.onClick.AddListener(Back);
    }

    void Update() {
      if (PhotonNetwork.CurrentRoom != null && !NetworkManager.instance.IsRoomReset() && !enteredRoom) {
        enteredRoom = true;
        NetworkManager.instance.ResetRoom();
        roomName.text = $"Room Name: {PhotonNetwork.CurrentRoom.Name}";
      }
      if (!initialized && NetworkManager.instance.IsRoomReset()) {
        initialized = true;
        Debug.Log("adding listener to ready button");
        toggleReadyButton.onClick.AddListener(ToggleReady);
      }
      if (initialized) {
        SetText();
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
      
      playerCounter.text = NetworkManager.instance.GetPlayers().Count.ToString();
    }

    void ToggleReady() {
      if (NetworkManager.instance.LocalPlayerPropertyIs<bool>("Ready", true)) {
        NetworkManager.instance.SetLocalPlayerProperty("Ready", false);
      } else {
        NetworkManager.instance.SetLocalPlayerProperty("Ready", true);
      }
    }
    
    void Back() {
      PhotonNetwork.LeaveRoom();
    }
    
    public override void OnLeftRoom() {
      joinRoomPage.Open();
    }
}
