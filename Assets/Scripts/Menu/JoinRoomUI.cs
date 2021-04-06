using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class JoinRoomUI: MonoBehaviourPunCallbacks {

    public TMP_InputField roomNameInput;
    public TMP_InputField playerNameInput;
    public Button createRoomButton; 
    public Button joinRoomButton;
    public Button createPrivateButton;
    public Button startButton;
    public Button findLobbiesButton;

    List<RoomInfo> roomList;

    public GameObject NameUI;
    public GameObject RoomUI;

    public GameObject FindLobbiesUI;
    public GameObject LobbyUI;

    public GameObject roomNamePrefab;
    public Transform gridLayout;

    void Start() {
        playerNameInput.Select();
        playerNameInput.ActivateInputField(); 
        createRoomButton.onClick.AddListener(OnClickCreateRoom);
        joinRoomButton.onClick.AddListener(OnClickJoinRoom);
        createPrivateButton.onClick.AddListener(OnClickCreatePrivateRoom);
        startButton.onClick.AddListener(StartButton);
        findLobbiesButton.onClick.AddListener(FindLobbiesButton);
    }

    public override void OnConnectedToMaster() {
        NameUI.SetActive(true);
        PhotonNetwork.JoinLobby();
    }

    void StartButton() {
        PhotonNetwork.LocalPlayer.NickName = playerNameInput.text;
        NameUI.SetActive(false);
        RoomUI.SetActive(true);  
        roomNameInput.Select();
        roomNameInput.ActivateInputField(); 
    }

    void FindLobbiesButton(){
        RoomUI.SetActive(false);
        FindLobbiesUI.SetActive(true);
        OnRoomListUpdate(roomList);
    }

    void OnClickCreateRoom() {
        Debug.Log("click registered");
        RoomUI.SetActive(false);
        LobbyUI.SetActive(true);
        NetworkManager.instance.CreateRoom(NetworkManager.instance.GenerateRoomName());
    }


    void OnClickJoinRoom() {
        RoomUI.SetActive(false);
        LobbyUI.SetActive(true);
        NetworkManager.instance.JoinRoom(roomNameInput.text);
    }

    void OnClickCreatePrivateRoom() {
        string PrivateRoom = 'p' + NetworkManager.instance.GenerateRoomName();
        RoomUI.SetActive(false);
        LobbyUI.SetActive(true);
        NetworkManager.instance.CreateRoom(PrivateRoom);
    }


    void OnClickJoinPrivateRoom() {
        NetworkManager.instance.JoinRoom('p' + NetworkManager.instance.GenerateRoomName());
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
            newRoom.GetComponentInChildren<Text>().text = room.Name; //+ "(" + room.PlayerCount + ")";
            newRoom.transform.SetParent(gridLayout);
        }
    }
}
