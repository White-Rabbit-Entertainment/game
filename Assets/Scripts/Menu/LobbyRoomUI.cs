using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class LobbyRoomUI : MonoBehaviour
{
  
    public Text playerList;
    
    public Button seekerButton;
    public Button robberButton;
    
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
      ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
      props.Add("PlayerTeam", "seeker");
      PhotonNetwork.LocalPlayer.SetCustomProperties(props);
      network.ChangeScene(gameScene);
    }

    void OnClickJoinRobber() {
      ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
      props.Add("PlayerTeam", "robber");
      PhotonNetwork.LocalPlayer.SetCustomProperties(props);
      network.ChangeScene(gameScene);
    }
}
