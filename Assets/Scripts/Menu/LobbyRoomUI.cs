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

    public GameObject robberPrefab;
    public GameObject seekerPrefab;
    
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
       seekerButton.onClick.AddListener(()=>OnJoin(seekerPrefab));
       robberButton.onClick.AddListener(()=>OnJoin(robberPrefab));
    }

    // Update is called once per frame
    void Update()
    {
      SetText();
    }

    void SetText() {
      playerList.text = networkManager.GetPlayers().Count.ToString();
    }

    void OnJoin(GameObject playerPrefab) {
      networkManager.ChangeScene(gameScene);
      PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(1,2,-10), Quaternion.identity);
      gameManager.OnStartGame();
    }
}
