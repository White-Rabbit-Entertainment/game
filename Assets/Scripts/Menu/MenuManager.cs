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

        #if UNITY_WEBGL
            webRTC.Initialize();
        #endif
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
        #if UNITY_WEBGL
            foreach (Player player in NetworkManager.instance.GetPlayers()) {
                if (PhotonNetwork.LocalPlayer != player) {
                    webRTC.Call(player.ActorNumber);
                }
            }
        #endif
    }

    public override void OnLeftRoom() {
        joinRoomPage.Open();
        
        #if UNITY_WEBGL
            foreach (Player player in NetworkManager.instance.GetPlayers()) {
                if (PhotonNetwork.LocalPlayer != player) {
                    webRTC.EndCall(player.ActorNumber);
                }
            }
        #endif
    }

    public override void OnPlayerLeftRoom(Player player) {
        #if UNITY_WEBGL
            webRTC.EndCall(player.ActorNumber);
        #endif
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
        this.roomList = roomList;
    }
}

