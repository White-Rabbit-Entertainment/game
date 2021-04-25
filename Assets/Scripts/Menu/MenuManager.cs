﻿using System.Collections;
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

    public WebRTC webRTC;
    
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
    
    public override void OnJoinedRoom() {
        lobbyPage.Open();
        foreach (Player player in NetworkManager.instance.GetPlayers()) {
            if (PhotonNetwork.LocalPlayer != Player) {
                webRTC.Call(player.ActorNumber);
            }
        }
    }

    // public override void OnPlayerEnteredRoom(Player player) {
    //     Debug.Log("Calling joined player");
    //     webRTC.Call(player.ActorNumber);
    // }
   
    public override void OnLeftRoom() {
        joinRoomPage.Open();
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
        this.roomList = roomList;
    }
}

