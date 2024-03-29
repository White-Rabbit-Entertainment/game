﻿using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Photon.Chat;     
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
    
/// <summary> <c>NetworkManager</c> handles logic to do with PhotonNetwork. It
/// is also a singleton see <c>GameManager</c> <see cref="GameManager"></see>
/// for more details. This is also initialized in the first scene. </summary>
public class NetworkManager : MonoBehaviourPunCallbacks {
    // Delegates to defined the required structure for functions to set room
    // properties.
    public delegate bool PhotonSetPropertyDelegate(Hashtable propertiesToSet, Hashtable expectedValues=null, WebFlags webFlags=null);
    public delegate void SetPropertyDelegate(string key, object value);

    // Singleton stuff see GameManager for details.
    public static NetworkManager instance;
    public static PlayableCharacter myCharacter;

    // Static chat client reference (shared between scenes)
    public static ChatClient chatClient;

    private string roomNameChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    void Start() {
      PhotonNetwork.ConnectUsingSettings();
    }

    /// <summary> Before a game is able to start various things need to be
    /// setup. Such as which team each player is on. </summary>
    public void SetupGame() {
      if (RoomPropertyIs<bool>("GameStarted", false)) {
        if (PhotonNetwork.LocalPlayer.IsMasterClient) {
          
          // When a game starts we dont want people to be able to join
          PhotonNetwork.CurrentRoom.IsVisible = false;
          
          // Set some game settings and properties
          SetRoomProperty("TasksSet", false);
          SetRoomProperty("WinningTeam", "None");
          SetRoomProperty("NumberOfTasks", 20);
          SetRoomProperty("NumberOfTasksInitiallyCompleted", 2);
          int numberOfTraitors = 1;
        
          // Get all the players in the room
          List<Player> players = GetPlayers();

          // Get all the player colours available
          List<GameObject> playerColorPrefabs = new List<GameObject>(Resources.LoadAll("PlayerPrefabs/Alive", typeof(GameObject)).Cast<GameObject>().ToArray());
          List<PlayerInfo> playerColors = new List<PlayerInfo>();
          foreach(GameObject playerColorPrefab in playerColorPrefabs) {
            playerColors.Add(playerColorPrefab.GetComponent<PlayerInfo>());
          }

          // Shuffle players and roles to ensure random team and role are assigned
          playerColors.Shuffle();
          players.Shuffle();

          // Assign all players a colour and team
          for (int i = 0; i < numberOfTraitors; i++) {
            SetPlayerProperty("Team", Team.Traitor, players[i]);
            SetPlayerProperty("Color", playerColors[i % playerColors.Count].assetPath, players[i]);
          }

          for (int i = numberOfTraitors; i < players.Count; i++) {
            SetPlayerProperty("Team", Team.Loyal, players[i]);
            SetPlayerProperty("Color", playerColors[i % playerColors.Count].assetPath, players[i]);
          }

          // Set the game as ready (i.e. setup complete)
          SetRoomProperty("GameReady", true);

        }
      }
    }
   
    // Generate a random 8 character name for a room 
    public string GenerateRoomName() {
        string roomString = string.Empty;
        for (int i = 0; i < 6; i++) {
            roomString += roomNameChars[UnityEngine.Random.Range(0, roomNameChars.Length)].ToString();
        }
        return roomString;
    }
    

    void Awake() {
      // Singleton stuff see GameManager for details.
      if (instance != null && instance != this) {
        gameObject.SetActive(false);
      }
      else { 
        instance = this;
        DontDestroyOnLoad(gameObject);
      }
    }

    // A call back for when user connects to the server.
    public override void OnConnectedToMaster() {
      // Debug.Log(PhotonNetwork.CloudRegion);
    }
    
    // A call back for when user creates a room. 
    public override void OnCreatedRoom() {}
    
    // A call back for when user joins a room. 
    public override void OnJoinedRoom() {
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> rooms) {}

    /* Helper to set custom properties, all examples are given for room
     * properties, but functions also exist for local player and player. */

    /// <summary> Function to get a value in custom properties. </summary>
    /// <example> For example:
    /// <code>
    ///    GetRoomProperty<bool>("GameStarted");
    /// </code>
    /// This returns the boolean value for GameStarted in the room
    /// custom properties. If it is not set it returns the default for boolean.
    /// </example>
    /// <example> A custom value can also be specified:
    /// <code>
    ///    GetRoomProperty<bool>("GameStarted", false);
    /// </code>
    /// This returns the boolean value for GameStarted in the room
    /// custom properties. In this case though if it is not set it will return
    /// false.
    /// </example>
    public T GetProperty<T>(string key, Hashtable properties, T defaultValue=default(T)) {
      object temp;
      if (typeof(T).IsEnum) {
        if (properties.TryGetValue(key, out temp) && temp is int) {
          T propertiesValue = (T)temp;
          return propertiesValue;
        }
      }
      if (properties.TryGetValue(key, out temp) && temp is T) {
          T propertiesValue = (T)temp;
          return propertiesValue;
      }
      return defaultValue;
    }

    /// <summary> Function to verify if a value in custom properties is equal
    /// to a given value.</summary>
    /// <example> For example:
    /// <code>
    ///    RoomPropertyIs<bool>("GameStarted", true);
    /// </code>
    /// This returns true if the game has started (ie gamestarted set to true
    /// in room).
    /// </example>
    public bool PropertyIs<T>(string key, T value, Hashtable properties) {
      return EqualityComparer<T>.Default.Equals(GetProperty<T>(key, properties), value);
    }
    
    /// <summary> Function to check if a value is in a given hashset </summary>
    /// <example> For example:
    /// <code>
    ///    RoomPropertyIs<bool>("GameStarted", true);
    /// </code>
    /// This returns true if the game has started (ie gamestarted set to true
    /// in room).
    /// </example>
    public bool HasProperty(string key, Hashtable properties) {
      return properties.ContainsKey(key);
    }
    
    /// <summary> Function to set value in custom properties. </summary>
    /// <example> For example:
    /// <code>
    ///    SetRoomProperty("GameStarted", true);
    /// </code>
    /// This sets the room property "GameStarted" to true for all clients.
    /// </example>
    public void SetProperty(string key, object value, Hashtable currentProperties, PhotonSetPropertyDelegate setProperties) {
      currentProperties[key] = value;
      setProperties(currentProperties);
    }
    
    /// <summary> Increment property value. If the value does not exist
    /// set it to 1. </summary>
    /// <example> For example:
    /// <code>
    ///    IncrementRoomProperty("NumberOfRobbers");
    /// </code>
    /// If the number of robbers is currently 2, the number will become 3.
    /// If the number of robbers is not set, the number will become 1.
    /// </example>
    public void IncrementProperty(string key, Hashtable properties, SetPropertyDelegate setProperty) {
      if (properties.ContainsKey(key)) {
        SetRoomProperty(key, (int)properties[key] + 1);
      } else {
        SetRoomProperty(key, 1);
      }
    }
    
    // Generic function to get value from CustomProperties (of Photon room or player)
    public T GetRoomProperty<T>(string key, T defaultValue = default(T)) {
      if (PhotonNetwork.CurrentRoom == null) {
        return defaultValue;
      }
      return GetProperty<T>(key, PhotonNetwork.CurrentRoom.CustomProperties, defaultValue);
    }

    public T GetPlayerProperty<T>(string key, Player player, T defaultValue = default(T)) {
      return GetProperty<T>(key, player.CustomProperties, defaultValue);
    }

    public T GetLocalPlayerProperty<T>(string key, T defaultValue = default(T)) {
      return GetProperty<T>(key, PhotonNetwork.LocalPlayer.CustomProperties, defaultValue);
    }

    public void SetRoomProperty(string key, object value) {
      SetProperty(key, value, PhotonNetwork.CurrentRoom.CustomProperties, PhotonNetwork.CurrentRoom.SetCustomProperties);
    }

    public void SetLocalPlayerProperty(string key, object value) {
      SetProperty(key, value, PhotonNetwork.LocalPlayer.CustomProperties, PhotonNetwork.LocalPlayer.SetCustomProperties);
    }

    public void SetPlayerProperty(string key, object value, Player player) {
      SetProperty(key, value, player.CustomProperties, player.SetCustomProperties);
    }

    public void IncrementRoomProperty(string key) {
      IncrementProperty(key, PhotonNetwork.CurrentRoom.CustomProperties, SetRoomProperty);
    }

    public void IncrementLocalPlayerProperty(string key) {
      IncrementProperty(key, PhotonNetwork.LocalPlayer.CustomProperties, SetLocalPlayerProperty);
    }
  
    public bool RoomPropertyIs<T>(string key, T value) {
      if (PhotonNetwork.CurrentRoom == null) {
        return false;
      }
      return PropertyIs<T>(key, value, PhotonNetwork.CurrentRoom.CustomProperties);
    }

    public bool LocalPlayerPropertyIs<T>(string key, T value) {
      return PlayerPropertyIs<T>(key, value, PhotonNetwork.LocalPlayer);
    }

    public bool PlayerPropertyIs<T>(string key, T value, Player player) {
      return PropertyIs<T>(key, value, player.CustomProperties);
    }
    
    public bool PlayerHasProperty(string key, Player player) {
      return HasProperty(key, player.CustomProperties);
    }
    
    // Check all players in the room and returns whether all the robbers are captured
    public bool NoLoyalsRemaining() {
      return !CheckAnyPlayers<Team>("Team", Team.Loyal);
    }

    public bool NoTraitorsRemaining() {
      return !CheckAnyPlayers<Team>("Team", Team.Traitor);
    }

    public int NumberOfTeamRemaining(Team team) {
      int count = 0;
      foreach (Player player in GetPlayers()) {
        if(PlayerPropertyIs<Team>("Team", team, player)) {
          count++;
        }
      }
      return count;
    }

    // Return true is all players have readied up.
    public bool AllPlayersReady() {
      return CheckAllPlayers<bool>("Ready", true);
    }

    //Return true if all players have been spawned into the game.
    public bool AllCharactersSpawned() {
      return CheckAllPlayers<bool>("Spawned", true);
    }

    public bool CheckAnyPlayers<T>(string key, T expectedValue) {
      foreach (Player player in GetPlayers()) {
          if (PlayerPropertyIs<T>(key, expectedValue, player)) {
              return true;
          }
      }
      return false;
    }

    public bool CheckAllPlayers<T>(string key, T expectedValue) {
      foreach (Player player in GetPlayers()) {
          if (!PlayerPropertyIs<T>(key, expectedValue, player)) {
              return false;
          }
      }
      return true;
    }
    // After a game resets various room and player properties. Not all
    // properties can be reset here (the game must still be over) so that the
    // other players leave the room. The other properties are set in SetupGame
    // in GameManager instead.
    public void ResetRoom() {
      SetRoomProperty("GameReady", false);
      SetRoomProperty("GameStarted", false);
      Timer.roundTimer.End();

      SetLocalPlayerProperty("CurrentScene", "LobbyScene");
      SetLocalPlayerProperty("Ready", false);
      SetLocalPlayerProperty("Spawned", false);
    }

    public bool IsRoomReset() {
      return CheckAnyPlayers("Ready", false) 
        && RoomPropertyIs<bool>("GameReady", false)
        && RoomPropertyIs<bool>("GameStarted", false)
        && CheckAllPlayers<string>("CurrentScene", "LobbyScene")
        && CheckAllPlayers<bool>("Spawned", false)
        && !Timer.roundTimer.IsStarted();
    } 

    public void CreateRoom (bool isVisible = true) {
      PhotonNetwork.CreateRoom(GenerateRoomName(), new RoomOptions {IsVisible = isVisible, EmptyRoomTtl = 1,});
    }

    public void JoinRoom(string roomName) {
      PhotonNetwork.JoinRoom(roomName);
    }
    
    public void ChangeScene(string sceneName) {
      PhotonNetwork.LoadLevel(sceneName);
    }
   
    // Returns a list of all the players in the current room
    public List<Player> GetPlayers() {
      // If the room is null return empty list
      if (PhotonNetwork.CurrentRoom == null) {
        return new List<Player>();
      }
      Dictionary<int, Player> players = PhotonNetwork.CurrentRoom.Players;
      return new List<Player>(players.Values);
    }

    public PlayableCharacter GetMe() {
      return myCharacter;
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) {
      Debug.Log(propertiesThatChanged);
    }
}
