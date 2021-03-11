using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class JoinRoomUI: MonoBehaviourPunCallbacks {

    public InputField roomNameInput;
    public InputField playerNameInput;
    public Button createRoomButton; 
    public Button joinRoomButton;
    public Button createPrivateButton;
    public Button startButton;

    public GameObject NameUI;
    public GameObject RoomUI;
    

    public static JoinRoomUI Instance;


    void Awake()
    {
        Instance = this;
    }


    public string RoomName()
    {
        int number;
        char code;
        string roomString = string.Empty;
        //Random random = new Random();
        for (int i = 0; i < 8; i++)
        {
            number = Random.Range(0, 10);
            if (number % 2 == 0)
                code = (char)('0' + (char)(number % 10));
            else
                code = (char)('A' + (char)(number % 26));
            roomString += code.ToString();
        }

        return roomString;
    }

    void Start() {
        PhotonNetwork.ConnectUsingSettings();


        createRoomButton.onClick.AddListener(OnClickCreateRoom);
        joinRoomButton.onClick.AddListener(OnClickJoinRoom);
        createPrivateButton.onClick.AddListener(OnClickCreatePrivateRoom);
        startButton.onClick.AddListener(StartButton);
    }

    public override void OnConnectedToMaster()
    {
        NameUI.SetActive(true);
        Debug.Log("Connected to the Master");
        PhotonNetwork.JoinLobby();

    }

    void StartButton()
    {
        NameUI.SetActive(false);
        RoomUI.SetActive(true);
    }

    void OnClickCreateRoom() {
      NetworkManager.instance.CreateRoom(RoomName());
      PhotonNetwork.LocalPlayer.NickName = playerNameInput.text;
    }


    void OnClickJoinRoom() {
      NetworkManager.instance.JoinRoom(roomNameInput.text);
      PhotonNetwork.LocalPlayer.NickName = playerNameInput.text;
    }

    void OnClickCreatePrivateRoom()
    {
        string PrivateRoom = 'p' + RoomName();
        NetworkManager.instance.CreateRoom(PrivateRoom);
    }


    void OnClickJoinPrivateRoom()
    {
        NetworkManager.instance.JoinRoom('p' + RoomName());
        PhotonNetwork.LocalPlayer.NickName = playerNameInput.text;
    }


    public void JoinRoom(string room)
    {
        PhotonNetwork.JoinRoom(room);
    }
}
