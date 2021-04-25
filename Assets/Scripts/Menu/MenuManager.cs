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

        // Call all the players in the room
        foreach (Player player in NetworkManager.instance.GetPlayers()) {
            if (PhotonNetwork.LocalPlayer != player) {
                webRTC.Call(player.ActorNumber);
            }
        }
    }

    public override void OnLeftRoom() {
        joinRoomPage.Open();
        foreach (Player player in NetworkManager.instance.GetPlayers()) {
            if (PhotonNetwork.LocalPlayer != player) {
                webRTC.EndCall(player.ActorNumber);
            }
        }
    }

    public override void OnPlayerLeftRoom(Player player) {
        webRTC.EndCall(player.ActorNumber);
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
        this.roomList = roomList;
    }
}

