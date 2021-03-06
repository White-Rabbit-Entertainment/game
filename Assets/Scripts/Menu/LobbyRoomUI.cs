﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyRoomUI : MonoBehaviourPunCallbacks {
    public Text playerCounter;

    public GameObject playerList;
    public Button toggleReadyButton;
    public GameObject readyPlayerItemPrefab;
    public GameObject unreadyPlayerItemPrefab;


    private Hashtable props;

    // Clears a list by destorying all children 
    public static void Clear(GameObject gameObject) {
      foreach (Transform child in gameObject.transform) {
        Destroy(child.gameObject);
      }
    }

    void Start() {
      Cursor.lockState = CursorLockMode.None;
      toggleReadyButton.onClick.AddListener(()=>toggleReady());
    }

    void Update() {
      SetText();
      if (NetworkManager.instance.AllPlayersReady()) {
        GameManager.instance.SetupGame();
        if (NetworkManager.instance.RoomPropertyIs<bool>("GameReady", true)) {
          NetworkManager.instance.SetRoomProperty("GameStarted", true);
          GameManager.instance.StartGame();
          Destroy(this);
        }
      }
    }

    
    void SetText() {
      Clear(playerList);
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

    void toggleReady() {
      if (NetworkManager.instance.LocalPlayerPropertyIs<bool>("Ready", true)) {
        NetworkManager.instance.SetLocalPlayerProperty("Ready", false);
      } else {
        NetworkManager.instance.SetLocalPlayerProperty("Ready", true);
      }
    }
}
