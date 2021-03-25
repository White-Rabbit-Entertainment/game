using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;

public class LobbyRoomUI : MonoBehaviourPunCallbacks {
    public Text playerCounter;

    public GameObject playerList;
    public Button toggleReadyButton;
    public GameObject readyPlayerItemPrefab;
    public GameObject unreadyPlayerItemPrefab;


    void Start() {
      Cursor.lockState = CursorLockMode.None;
      Cursor.visible = true;
      toggleReadyButton.onClick.AddListener(ToggleReady);
    }

    void Update() {
      SetText();
      Debug.Log(PhotonNetwork.LocalPlayer.CustomProperties.ToStringFull());
      if (NetworkManager.instance.AllPlayersReady()) {
        NetworkManager.instance.SetupGame();
        if (NetworkManager.instance.RoomPropertyIs<bool>("GameReady", true)) {
          NetworkManager.instance.SetRoomProperty("GameStarted", true);
          NetworkManager.instance.ChangeScene("GameScene");
          Destroy(this);
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
      if (NetworkManager.instance.LocalPlayerPropertyIs<string>("Ready", "true")) {
        NetworkManager.instance.SetLocalPlayerProperty("Ready", "false");
      } else {
        NetworkManager.instance.SetLocalPlayerProperty("Ready", "true");
      }
    }
}
