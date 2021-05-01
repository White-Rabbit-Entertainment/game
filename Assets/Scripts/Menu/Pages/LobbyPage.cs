using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyPage : MenuPage {
    public Text playerCounter;
    public Text roomName;

    public Dictionary<Player, GameObject> playerTiles = new Dictionary<Player, GameObject>();
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
      playerTiles = new Dictionary<Player, GameObject>();
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
    }

    public override void OnPlayerLeftRoom(Player player) {
      Debug.Log(playerTiles.ToStringFull());
      Destroy(playerTiles[player]);
      playerTiles.Remove(player);
    }

    public override void OnPlayerEnteredRoom(Player player) {
      AddTile(player);
    }

    void AddTile(Player player) {
      GameObject item = Instantiate(playerTilePrefab, transform);
      Debug.Log("adding playerTile to disct");
      playerTiles.Add(player, item);
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

    public override void OnPlayerPropertiesUpdate(Player player, Hashtable changedProperties) {
      Debug.Log("player properties update");
      if (changedProperties.ContainsKey("Ready")) {
        if (NetworkManager.instance.GetProperty<bool>("Ready", changedProperties)) {
          playerTiles[player].GetComponent<PlayerTile>().DisableVotingAgainst();
          playerTiles[player].GetComponent<PlayerTile>().EnableVotingFor();
        } else {
          playerTiles[player].GetComponent<PlayerTile>().DisableVotingFor();
          playerTiles[player].GetComponent<PlayerTile>().EnableVotingAgainst();
        }
      }
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
