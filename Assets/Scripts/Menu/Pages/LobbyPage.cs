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

    public Dictionary<int, GameObject> playerTiles = new Dictionary<int, GameObject>();
    public GameObject playerList;
    public Button toggleReadyButton;
    public GameObject playerTilePrefab;
    [SerializeField] private ChatManager chatManager;

    public JoinRoomPage joinRoomPage;
    public Button back;

    bool initialized;
    bool enteredRoom; 

    void Start() {
      toggleReadyButton.onClick.AddListener(ToggleReady);
    }

    void OnEnable() {
      Cursor.lockState = CursorLockMode.None;
      Cursor.visible = true;
      initialized = false;
      enteredRoom = false;
      back.onClick.AddListener(Back);
      roomName.text = $"Room Name: {PhotonNetwork.CurrentRoom.Name}";
      chatManager.JoinRoomChat(PhotonNetwork.CurrentRoom);
      playerTiles = new Dictionary<int, GameObject>();
    }

    void Update() {
      if (PhotonNetwork.CurrentRoom != null && !NetworkManager.instance.IsRoomReset() && !enteredRoom) {
        enteredRoom = true;
        NetworkManager.instance.ResetRoom();
      }
      if (!initialized && NetworkManager.instance.IsRoomReset()) {
        initialized = true;
      }
      if (initialized) {
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

    public override void OnJoinedRoom() {
      foreach (Player player in NetworkManager.instance.GetPlayers()) {
        AddTile(player);
      }
    }

    public override void OnLeftRoom() {
      Debug.Log("DestroyingChildren");
      playerList.DestroyChildren();
      playerTiles = null;
      // Destroy(playerTiles[PhotonNetwork.LocalPlayer.ActorNumber]);
      // playerTiles.Remove(PhotonNetwork.LocalPlayer.ActorNumber);
      // foreach (Player player in NetworkManager.instance.GetPlayers()) {
      //   Destroy(playerTiles[player.ActorNumber]);
      //   playerTiles.Remove(player.ActorNumber);
      // }
    }

    public override void OnPlayerLeftRoom(Player player) {
      Debug.Log(playerTiles.ToStringFull());
      Destroy(playerTiles[player.ActorNumber]);
      playerTiles.Remove(player.ActorNumber);
    }

    public override void OnPlayerEnteredRoom(Player player) {
      AddTile(player);
    }

    void AddTile(Player player) {
      GameObject item = Instantiate(playerTilePrefab, transform);
      Debug.Log("adding playerTile to disct");
      playerTiles.Add(player.ActorNumber, item);
      item.GetComponent<PlayerTile>().Init(player.NickName, new Color(255,0,0));
      item.transform.SetParent(playerList.transform);
      item.GetComponent<PlayerTile>().Start();
      if (NetworkManager.instance.PlayerPropertyIs("Ready", true, player)) {
        item.GetComponent<PlayerTile>().EnableVotingFor();
      } else {
        item.GetComponent<PlayerTile>().EnableVotingAgainst();
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
      NetworkManager.instance.SetLocalPlayerProperty("Ready", false);
      chatManager.LeaveRoomChat(PhotonNetwork.CurrentRoom);
      PhotonNetwork.LeaveRoom(false);
    }
}
