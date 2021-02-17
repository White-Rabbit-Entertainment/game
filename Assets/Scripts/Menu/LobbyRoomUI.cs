﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyRoomUI : MonoBehaviourPun {
    public Text playerCounter;
    public GameObject playerList;
    public Button toggleReadyButton;
    public GameObject robberPrefab;
    public GameObject seekerPrefab;
    public GameObject readyPlayerItemPrefab;
    public GameObject unreadyPlayerItemPrefab;

    private bool gameStarted = false; 
    private Hashtable props;

    void Start() {
      toggleReadyButton.onClick.AddListener(()=>toggleReady());
    }

    void Update() {
      SetText();
      if (NetworkManager.instance.AllPlayersReady() && !gameStarted) {
        GameManager.instance.SetupGame();
        if (NetworkManager.instance.RoomPropertyIs<bool>("GameReady", true)) {
          gameStarted = true;
          GameManager.instance.StartGame();
        }
      }
    }

    void SetText() {
      foreach (Transform child in playerList.transform) {
        Destroy(child.gameObject);
      }
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
