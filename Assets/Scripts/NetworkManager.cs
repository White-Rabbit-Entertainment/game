using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // Instance
    public static NetworkManager instance;
    private string lobbyScene = "LobbyScene";

    void Awake()
    {
      // If there is already a network manager instance then stop
      if (instance != null && instance != this) {
        gameObject.SetActive(false);
      }
      else { 
        // Otherwise set the instance to this class 
        instance = this;

        // When we change scenes (eg to game scene) dont desotry this instance
        DontDestroyOnLoad(gameObject);
      }
    }

    void Start() {
      PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() {
      Debug.Log("Connected to master server");
      // CreateRoom("testRoom");
    }
    
    public override void OnCreatedRoom() {
      Debug.Log("Room created: " + PhotonNetwork.CurrentRoom.Name);
    }
    
    public override void OnJoinedRoom() {
      Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom.Name);
      SceneManager.LoadScene(lobbyScene);
    }

    public void CreateRoom (string roomName) {
      PhotonNetwork.CreateRoom(roomName);
    }

    public void JoinRoom(string roomName) {
      PhotonNetwork.JoinRoom(roomName);
    }
    
    public void ChangeScene(string sceneName) {
      PhotonNetwork.LoadLevel(sceneName);
    }
}
