using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class JoinRoomUI: MonoBehaviour {

    public InputField roomNameInput;
    public InputField playerNameInput;
    public Button createRoomButton; 
    public Button joinRoomButton; 

    void Start() {
      createRoomButton.onClick.AddListener(OnClickCreateRoom);
      joinRoomButton.onClick.AddListener(OnClickJoinRoom);
    }

    void OnClickCreateRoom() {
      NetworkManager.instance.CreateRoom(roomNameInput.text);
      PhotonNetwork.LocalPlayer.NickName = playerNameInput.text;
    }
    
    void OnClickJoinRoom() {
      NetworkManager.instance.JoinRoom(roomNameInput.text);
      PhotonNetwork.LocalPlayer.NickName = playerNameInput.text;
    }
}
