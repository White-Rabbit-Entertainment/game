﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // Instance
    public static NetworkManager instance;
    private string lobbyScene = "LobbyScene";

    void Awake()
    {
      // If there is already a network manager instance then stop
      if (instance != null && instance != this) {
        gameObject.SetActive(false);
      }
      else { 
        // Otherwise set the instance to this class 
        instance = this;

        // When we change scenes (eg to game scene) dont desotry this instance
        DontDestroyOnLoad(gameObject);
      }
    }

    void Start() {
      PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() {
      Debug.Log("Connected to master server");
      // CreateRoom("testRoom");
    }
    
    public override void OnCreatedRoom() {
      Debug.Log("Room created: " + PhotonNetwork.CurrentRoom.Name);
    }
    
    public override void OnJoinedRoom() {
      Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom.Name);
      ChangeScene(lobbyScene);
    }

    public void SetRoomProperty(string key, object value) {
      Hashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;
      if (properties.ContainsKey(key)) {
        properties[key] = value;
      } else {
        properties.Add(key, value);
      }
      PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
    }

    public void SetLocalPlayerProperty(string key, object value) {
      Hashtable properties = PhotonNetwork.LocalPlayer.CustomProperties;
      if (properties.ContainsKey(key)) {
        properties[key] = value;
      } else {
        properties.Add(key, value);
      }
      PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
    }

    public void IncrementRoomProperty(string key) {
      Hashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;
      if (properties.ContainsKey(key)) {
        SetRoomProperty(key, (int)properties[key] + 1);
      } else {
        SetRoomProperty(key, 1);
      }
    }

    public void IncrementLocalPlayerProperty(string key) {
      Hashtable properties = PhotonNetwork.LocalPlayer.CustomProperties;
      if (properties.ContainsKey(key)) {
        SetRoomProperty(key, (int)properties[key] + 1);
      } else {
        SetRoomProperty(key, 1);
      }
    }

    public bool RoomPropertyIs<T>(string key, T value) {
      Hashtable properties = PhotonNetwork.LocalPlayer.CustomProperties;
      object temp;
      if (properties.TryGetValue(key, out temp) && temp is T) {
          T propertiesValue = (T)temp;
          return (EqualityComparer<T>.Default.Equals(propertiesValue, value));
      }
      return false;
    }

    public bool LocalPlayerPropertyIs<T>(string key, T value) {
      return PlayerPropertyIs<T>(key, value, PhotonNetwork.LocalPlayer);
    }

    public bool PlayerPropertyIs<T>(string key, T value, Player player) {
      Hashtable properties = player.CustomProperties;
      object temp;
      if (properties.TryGetValue(key, out temp) && temp is T) {
          T propertiesValue = (T)temp;
          return (EqualityComparer<T>.Default.Equals(propertiesValue, value));
      }
      return false;
    }

    public void StartRoundTimer(double roundLength) {
      Debug.Log("Started Timer");
      SetRoomProperty("RoundLength", roundLength);
      SetRoomProperty("RoundTimerStart", PhotonNetwork.Time);
    }

    public double GetRoundTimeRemaining() {
      return (double)PhotonNetwork.CurrentRoom.CustomProperties["RoundLength"] - (PhotonNetwork.Time - (double)PhotonNetwork.CurrentRoom.CustomProperties["RoundTimerStart"]);
    }

    public bool AllRobbersCaught() {
      bool allRobbersCaught = true;
      foreach (Player player in GetPlayers()) {
          if (PlayerPropertyIs<string>("Team", "Robber", player) && (!PlayerPropertyIs<bool>("Captured", true, player))) {
              allRobbersCaught = false;
          }
      }
      return allRobbersCaught;
    }

    public void CreateRoom (string roomName) {
      PhotonNetwork.CreateRoom(roomName);
    }

    public void JoinRoom(string roomName) {
      PhotonNetwork.JoinRoom(roomName);
    }
    
    public void ChangeScene(string sceneName) {
      PhotonNetwork.LoadLevel(sceneName);
    }
    
    public List<Player> GetPlayers() {
      Dictionary<int, Player> players = PhotonNetwork.CurrentRoom.Players;
      return new List<Player>(players.Values);
    }
}