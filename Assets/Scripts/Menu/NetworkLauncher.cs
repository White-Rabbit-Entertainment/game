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

    public GameObject roomListUI;

    public override void OnConnectedToMaster() {
        nameUI.SetActive(true);
        PhotonNetwork.JoinLobby();
    }

    public void JoinOrCreateButton() {
        roomListUI.SetActive(false);
        loginUI.SetActive(false);

        PhotonNetwork.JoinOrCreateRoom(NetworkManager.instance.GenerateRoomName(), new RoomOptions {}, default);
    }
}
