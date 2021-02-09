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
    private NetworkManager network;
    private string gameScene = "GameScene";

    // Start is called before the first frame update
    void Start()
    {
       network = new NetworkManager();
        
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
      playerList.text = network.GetPlayers().Count.ToString();
    }

    void OnClickJoinSeeker() {
      props = new Hashtable();
      props.Add("PlayerTeam", "seeker");
      PhotonNetwork.LocalPlayer.NickName = "seeker";
      PhotonNetwork.LocalPlayer.SetCustomProperties(props);
      Debug.Log(PhotonNetwork.LocalPlayer.CustomProperties["PlayerTeam"]);
      foreach(var key in props.Keys) {
            Debug.Log(String.Format("{0}: {1}", key, props[key]));
      }
      foreach(var key in PhotonNetwork.LocalPlayer.CustomProperties.Keys) {
            Debug.Log(String.Format("{0}: {1}", key, PhotonNetwork.LocalPlayer.CustomProperties[key]));
      }
      network.ChangeScene(gameScene);
    }

    void OnClickJoinRobber() {
      props = new Hashtable();
      props.Add("PlayerTeam", "robber");
      PhotonNetwork.LocalPlayer.NickName = "robber";
      PhotonNetwork.LocalPlayer.SetCustomProperties(props);
      foreach(var key in props.Keys) {
            Debug.Log(String.Format("{0}: {1}", key, props[key]));
        }
      network.ChangeScene(gameScene);
    }
}
