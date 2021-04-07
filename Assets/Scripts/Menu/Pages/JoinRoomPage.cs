using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class JoinRoomPage : MenuPage {

    public TMP_InputField roomNameInput;
    public Button createRoomButton; 
    public Button joinRoomButton;
    public Button createPrivateButton;
    public Button findLobbiesButton;

    public RoomListPage roomListPage;
    public LobbyPage lobbyPage;

    void Start() {
        createRoomButton.onClick.AddListener(OnClickCreateRoom);
        joinRoomButton.onClick.AddListener(OnClickJoinRoom);
        createPrivateButton.onClick.AddListener(OnClickCreatePrivateRoom);
        findLobbiesButton.onClick.AddListener(roomListPage.Open);
        
        roomNameInput.Select();
        roomNameInput.ActivateInputField(); 
    }

    void OnClickCreateRoom() {
        NetworkManager.instance.CreateRoom(NetworkManager.instance.GenerateRoomName());
    }

    void OnClickJoinRoom() {
        NetworkManager.instance.JoinRoom(roomNameInput.text);
    }

    void OnClickCreatePrivateRoom() {
        string PrivateRoom = 'p' + NetworkManager.instance.GenerateRoomName();
        NetworkManager.instance.CreateRoom(PrivateRoom);
    }

    void OnClickJoinPrivateRoom() {
        NetworkManager.instance.JoinRoom('p' + NetworkManager.instance.GenerateRoomName());
    }
}
