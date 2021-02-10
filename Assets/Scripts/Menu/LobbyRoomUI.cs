using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyRoomUI : MonoBehaviourPun
{
  
    public Text playerList;
    
    public Button seekerButton;
    public Button robberButton;
    
    private Hashtable props;
    private NetworkManager networkManager; 
    private GameManager gameManager;
    private string gameScene = "GameScene";

    // Start is called before the first frame update
    void Start()
    {
       networkManager = new NetworkManager();
       gameManager = new GameManager();
        
       // Setup start game button
       seekerButton.onClick.AddListener(OnClickJoinSeeker);
       robberButton.onClick.AddListener(OnClickJoinRobber);
    }

    // Update is called once per frame
    void Update()
    {
      SetText();
    }

    void SetText() {
      playerList.text = networkManager.GetPlayers().Count.ToString();
    }

    void OnJoin() {
      networkManager.ChangeScene(gameScene);
      gameManager.OnStartGame();
    }

    void OnClickJoinSeeker() {
      props = new Hashtable();
      props.Add("PlayerTeam", "seeker");
      PhotonNetwork.LocalPlayer.NickName = "seeker";
      PhotonNetwork.LocalPlayer.SetCustomProperties(props);
      OnJoin();
    }

    void OnClickJoinRobber() {
      props = new Hashtable();
      props.Add("PlayerTeam", "robber");
      PhotonNetwork.LocalPlayer.NickName = "robber";
      PhotonNetwork.LocalPlayer.SetCustomProperties(props);
      OnJoin();
    }
}
