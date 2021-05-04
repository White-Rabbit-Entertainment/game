using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class MenuManager: MonoBehaviourPunCallbacks {

    public MenuPage currentPage;
    public NameInputPage nameInputPage;
    public LobbyPage lobbyPage;
    public JoinRoomPage joinRoomPage;

    public WebRTC webRTC;
    
    private TypedLobby lobby = new TypedLobby(null, LobbyType.Default);
    public Dictionary<string, RoomInfo> roomList = new Dictionary<string, RoomInfo>();

    void Start() {

        webRTC.Initialize();
        if (PhotonNetwork.LocalPlayer.NickName == null || PhotonNetwork.LocalPlayer.NickName == "") {
            nameInputPage.Open();
        } else if (PhotonNetwork.CurrentRoom != null) {
            lobbyPage.Open();
        } else {
            joinRoomPage.Open();
        }
    }

    public override void OnConnectedToMaster() {
        PhotonNetwork.JoinLobby(lobby);
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomUpdatesList) {
        for(int i=0; i< roomUpdatesList.Count; i++) {
            RoomInfo info = roomUpdatesList[i];
            if (info.RemovedFromList) {
                roomList.Remove(info.Name);
            } else {
                roomList[info.Name] = info;
            }
        }
    }

    public override void OnJoinedLobby() {
        roomList.Clear();
    }
    
    public override void OnLeftLobby() {
        roomList.Clear();
    }

    public override void OnDisconnected(DisconnectCause cause) {
        roomList.Clear();
    }
    
    public override void OnJoinedRoom() {
        lobbyPage.Open();
        currentPage.OnJoinedRoom();

        // Call all the players in the room
        foreach (Player player in NetworkManager.instance.GetPlayers()) {
            if (PhotonNetwork.LocalPlayer != player) {
                webRTC.Call(player.ActorNumber);
            }
        }
    }

    public override void OnLeftRoom() {
        Debug.Log("On left room");
        currentPage.OnLeftRoom();
        joinRoomPage.Open();
        PhotonNetwork.JoinLobby();
        base.OnLeftRoom();
    }

    public override void OnPlayerPropertiesUpdate(Player player, Hashtable changedProperties) {
        currentPage.OnPlayerPropertiesUpdate(player, changedProperties);
    }

    public override void OnPlayerEnteredRoom(Player player) {
        currentPage.OnPlayerEnteredRoom(player);
    }

    public override void OnPlayerLeftRoom(Player player) {
        currentPage.OnPlayerLeftRoom(player);
        webRTC.EndCall(player.ActorNumber);
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> roomUpdatesList) {
        UpdateCachedRoomList(roomUpdatesList);
        if (currentPage is RoomListPage) {
            ((RoomListPage)currentPage).OnCachedRoomListUpdate(roomList);
        }
    }
}

