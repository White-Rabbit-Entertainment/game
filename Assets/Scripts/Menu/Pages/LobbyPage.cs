using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyPage : MenuPage {
    public TMP_Text playerCounter;
    public TMP_Text roomName;

    public Dictionary<Player, GameObject> playerTiles = new Dictionary<Player, GameObject>();
    public GameObject playerList;
    public Button toggleReadyButton;
    public GameObject playerTilePrefab;
    [SerializeField] private ChatManager chatManager;
    [SerializeField] private WebRTC webRTC;
    public GameObject titleText;

    // Start game UI
    public Button startGameButton; 
    public GameObject startGameUI;

    public JoinRoomPage joinRoomPage;
    public Button back;

    public bool gameStarted = false;

    bool initialized;
    bool enteredRoom; 

    void Start() {
      toggleReadyButton.onClick.AddListener(ToggleReady);
      startGameButton.onClick.AddListener(StartGame);
    }

    void OnEnable() {
      Debug.Log("happening");
      Cursor.lockState = CursorLockMode.None;
      Cursor.visible = true;
      initialized = false;
      enteredRoom = false;
      back.onClick.AddListener(Back);
      roomName.text = $"Room Name: {PhotonNetwork.CurrentRoom.Name}";
      // chatManager.Init();
      chatManager.JoinRoomChat(PhotonNetwork.CurrentRoom);
      playerTiles = new Dictionary<Player, GameObject>();
     
      // Draw all the palyer tiles
      foreach (Player player in NetworkManager.instance.GetPlayers()) {
        AddTile(player);
      }
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
        // If all the players are ready then the master clint can start the game
        if (PhotonNetwork.IsMasterClient && NetworkManager.instance.AllPlayersReady()) {
          startGameUI.SetActive(true);
        } else if (!gameStarted) {
          startGameUI.SetActive(false);
        }
      }
    }

    public void StartGame() {
      if (!gameStarted) {
        gameStarted = true;
        NetworkManager.instance.SetupGame();
        if (NetworkManager.instance.RoomPropertyIs<bool>("GameReady", true)) {
          NetworkManager.instance.SetRoomProperty("GameStarted", true);
          GetComponent<PhotonView>().RPC("StartGameRPC", RpcTarget.All);
        }
      }
    }

    [PunRPC]
    public void StartGameRPC() {
      gameStarted = true;
      NetworkManager.instance.ChangeScene("GameScene");
    }

    public override void Open() {
      base.Open();
      titleText.SetActive(false);
    }

    public override void Close() {
      base.Close();
      titleText.SetActive(true);
    }

    public override void OnJoinedRoom() {}

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
      playerTiles.Add(player, item);
      item.GetComponent<PlayerTile>().Init(player.NickName, new Color(0,0,0));
      item.transform.SetParent(playerList.transform);
      item.GetComponent<PlayerTile>().Start();
      if (NetworkManager.instance.PlayerPropertyIs("Ready", true, player)) {
        item.GetComponent<PlayerTile>().EnableVotingFor();
      } else {
        item.GetComponent<PlayerTile>().EnableVotingAgainst();
      }
      playerCounter.text = $"Players: {NetworkManager.instance.GetPlayers().Count.ToString()}/10";
    }

    public override void OnPlayerPropertiesUpdate(Player player, Hashtable changedProperties) {
      Debug.Log("player properties update");
      if (changedProperties.ContainsKey("Ready")) {
        if (NetworkManager.instance.GetProperty<bool>("Ready", changedProperties)) {
          playerTiles[player].GetComponent<PlayerTile>().EnableVotingAgainst(false);
          playerTiles[player].GetComponent<PlayerTile>().EnableVotingFor();
        } else {
          playerTiles[player].GetComponent<PlayerTile>().EnableVotingFor(false);
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
      foreach (Player player in NetworkManager.instance.GetPlayers()) {
          if (PhotonNetwork.LocalPlayer != player) {
              webRTC.EndCall(player.ActorNumber);
          }
      }
      // NetworkManager.instance.SetLocalPlayerProperty("Ready", false);
      chatManager.LeaveRoomChat(PhotonNetwork.CurrentRoom);
      PhotonNetwork.LeaveLobby();
      PhotonNetwork.LeaveRoom();
    }
}
