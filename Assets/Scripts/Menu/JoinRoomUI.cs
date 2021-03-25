﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class JoinRoomUI: MonoBehaviourPunCallbacks {

    public InputField roomNameInput;
    public InputField playerNameInput;
    public Button createRoomButton; 
    public Button joinRoomButton;
    public Button createPrivateButton;
    public Button startButton;

    List<RoomInfo> roomList;

    public GameObject NameUI;
    public GameObject RoomUI;

    public GameObject roomNamePrefab;
    public Transform gridLayout;

    void Start() {
        createRoomButton.onClick.AddListener(OnClickCreateRoom);
        joinRoomButton.onClick.AddListener(OnClickJoinRoom);
        createPrivateButton.onClick.AddListener(OnClickCreatePrivateRoom);
        startButton.onClick.AddListener(StartButton);
    }

    public override void OnConnectedToMaster() {
        NameUI.SetActive(true);
        PhotonNetwork.JoinLobby();
    }

    void StartButton() {
        NameUI.SetActive(false);
        RoomUI.SetActive(true);
        OnRoomListUpdate(roomList);

    }

    void OnClickCreateRoom() {
      NetworkManager.instance.CreateRoom(NetworkManager.instance.GenerateRoomName());
      PhotonNetwork.LocalPlayer.NickName = playerNameInput.text;
    }


    void OnClickJoinRoom() {
      NetworkManager.instance.JoinRoom(roomNameInput.text, playerNameInput.text);
    }

    void OnClickCreatePrivateRoom() {
        string PrivateRoom = 'p' + NetworkManager.instance.GenerateRoomName();
        NetworkManager.instance.CreateRoom(PrivateRoom);
    }


    void OnClickJoinPrivateRoom() {
        NetworkManager.instance.JoinRoom('p' + NetworkManager.instance.GenerateRoomName(), playerNameInput.text);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
        this.roomList = roomList;
        for (int i = 0; i < gridLayout.childCount; i++) {
            if (gridLayout.GetChild(i).gameObject.GetComponentInChildren<Text>().text == roomList[i].Name) {
                Destroy(gridLayout.GetChild(i).gameObject);

                if (roomList[i].PlayerCount == 0) {
                    roomList.Remove(roomList[i]);
                }
            }
        }


        foreach (var room in roomList) {
            if (room.Name.StartsWith("p")) {
                continue;
            }
            
            GameObject newRoom = Instantiate(roomNamePrefab, gridLayout.position, Quaternion.identity);

            newRoom.GetComponent<RoomListItem>().playerName = playerNameInput.text;

            newRoom.GetComponentInChildren<Text>().text = room.Name; //+ "(" + room.PlayerCount + ")";
            newRoom.transform.SetParent(gridLayout);
        }
    }
}
