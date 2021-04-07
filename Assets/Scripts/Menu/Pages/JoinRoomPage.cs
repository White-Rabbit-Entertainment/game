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
        roomNameInput.Select();
        roomNameInput.ActivateInputField(); 

        createRoomButton.onClick.AddListener(() => NetworkManager.instance.CreateRoom(true));
        joinRoomButton.onClick.AddListener(() => NetworkManager.instance.JoinRoom(roomNameInput.text));
        createPrivateButton.onClick.AddListener(() => NetworkManager.instance.CreateRoom(false));
        findLobbiesButton.onClick.AddListener(roomListPage.Open);
    }
}
