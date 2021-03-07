using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkManager : MonoBehaviourPunCallbacks
{

    public delegate bool PhotonSetPropertyDelegate(Hashtable propertiesToSet, Hashtable expectedValues=null, WebFlags webFlags=null);
    public delegate void SetPropertyDelegate(string key, object value);

    // Instance
    public static NetworkManager instance;
    private string lobbyScene = "LobbyScene";

    void Awake()
    {
      if (instance != null && instance != this) {
        gameObject.SetActive(false);
      }
      else { 
        instance = this;
        DontDestroyOnLoad(gameObject);
      }
    }

    void Start() {
      PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() {
      Debug.Log("Connected to master server");
      Debug.Log(PhotonNetwork.CloudRegion);
    }
    
    public override void OnCreatedRoom() {
      Debug.Log("Room created: " + PhotonNetwork.CurrentRoom.Name);
    }
    
    public override void OnJoinedRoom() {
      Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom.Name);
      ChangeScene(lobbyScene);
    }

    // Generic function to get property from CustomProperties (of Photon room or player)
    public T GetProperty<T>(string key, Hashtable properties, T defaultValue=default(T)) {
      object temp;
      if (properties.TryGetValue(key, out temp) && temp is T) {
          T propertiesValue = (T)temp;
          return propertiesValue;
      }
      return defaultValue;
    }

    // Generic function to check value from CustomProperties (of Photon room or player)
    public bool PropertyIs<T>(string key, T value, Hashtable properties) {
      return EqualityComparer<T>.Default.Equals(GetProperty<T>(key, properties), value);
    }
    
    // Generic function to set value from CustomProperties (of Photon room or player)
    public void SetProperty(string key, object value, Hashtable currentProperties, PhotonSetPropertyDelegate setProperties) {
      currentProperties[key] = value;
      setProperties(currentProperties);
    }
    
    // Generic function to increment value from CustomProperties (of Photon room or player)
    // If the value is not set it is set to 1
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

    // Start the timer for the game, but assigning the start time and round length (which all clients use)
    public void StartRoundTimer(double roundLength) {
      SetRoomProperty("RoundLength", roundLength);
      SetRoomProperty("RoundTimerStart", PhotonNetwork.Time);
    }

    // Returns round time remaining (or 0 if not started)
    public double GetRoundTimeRemaining() {
      return GetRoomProperty<double>("RoundLength", 0f) - (PhotonNetwork.Time - GetRoomProperty<double>("RoundTimerStart", 0f));
    }

    // Check all players in the room and returns whether all the robbers are captured
    public bool AllRobbersCaught() {
      foreach (Player player in GetPlayers()) {
          if (PlayerPropertyIs<string>("Team", "Robber", player) && (!PlayerPropertyIs<bool>("Captured", true, player))) {
              return false;
          }
      }
      return true;
    }

    public bool AllPlayersReady() {
      foreach (Player player in GetPlayers()) {
          if (!PlayerPropertyIs<bool>("Ready", true, player)) {
              return false;
          }
      }
      return true;
    }

    public bool AllPlayersInGame() {
      foreach (Player player in GetPlayers()) {
          if (!PlayerPropertyIs<bool>("InGameScene", true, player)) {
              return false;
          }
      }
      return true;
    }

    public void ResetRoom() {
      SetRoomProperty("GameReady", false);
      SetRoomProperty("GameStarted", false);
      SetRoomProperty("ItemsStolen", 0);
      foreach(Player player in GetPlayers()) {
        SetPlayerProperty("Ready", false, player);
        SetPlayerProperty("InGameScene", false, player);
        SetPlayerProperty("Captured", false, player);
      }
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
   
    // Returns a list of all the players in the current room
    public List<Player> GetPlayers() {
      // If the room is null return empty list
      if (PhotonNetwork.CurrentRoom == null) {
        return new List<Player>();
      }
      Dictionary<int, Player> players = PhotonNetwork.CurrentRoom.Players;
      return new List<Player>(players.Values);
    }
}
