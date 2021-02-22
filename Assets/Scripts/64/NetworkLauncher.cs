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
    public InputField roomName;

    public GameObject roomListUI;
    

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
        if (roomName.text.Length < 2)
            return;
        loginUI.SetActive(false);

        RoomOptions options = new RoomOptions { MaxPlayers = 6 };
        PhotonNetwork.JoinOrCreateRoom(roomName.text, options, default);


    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(1);
    }

}
