using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;



public class NetworkLauncher : MonoBehaviourPunCallbacks
{
    public GameObject loginUI;
    public GameObject nameUI;
    public InputField playerName;
    //public InputField roomName;

    public GameObject roomListUI;



    public string roomName()
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
    

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        nameUI.SetActive(true);
        Debug.Log("Connected to the Master");
        PhotonNetwork.JoinLobby();

    }

    public void PlayButtom()
    {
        nameUI.SetActive(false);
        PhotonNetwork.NickName = playerName.text;
        loginUI.SetActive(true);
        if (PhotonNetwork.InLobby)
        {
            roomListUI.SetActive(true);
        }
    }


    public void JoinOrCreateButton()
    {
        //if (roomName.text.Length < 2)
        //    return;



        loginUI.SetActive(false);

        RoomOptions options = new RoomOptions { MaxPlayers = 6 };
        //PhotonNetwork.JoinOrCreateRoom(roomName.text, options, default);
        PhotonNetwork.JoinOrCreateRoom(roomName(), options, default);


    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(1);
    }

}
