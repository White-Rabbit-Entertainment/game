using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using TMPro;

public class MenuManager: MonoBehaviourPunCallbacks {

    public MenuPage currentPage;
    public NameInputPage nameInputPage;
    public LobbyPage lobbyPage;
    public JoinRoomPage joinRoomPage;
    
    public List<RoomInfo> roomList;

    void Start() {
        if (PhotonNetwork.LocalPlayer.NickName == null || PhotonNetwork.LocalPlayer.NickName == "") {
            nameInputPage.Open();
        } else if (PhotonNetwork.CurrentRoom != null) {
            lobbyPage.Open();
        } else {
            joinRoomPage.Open();
        }
    }

    public override void OnConnectedToMaster() {
        PhotonNetwork.JoinLobby();
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
        Debug.Log("On room list update manager");
        Debug.Log(roomList);
        Debug.Log(roomList.Count);
        this.roomList = roomList;
    }
}

