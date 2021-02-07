using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyRoomUI : MonoBehaviour
{

    public Text playerList;
    
    public Button startGame; 
    
    private NetworkManager network;
    private string gameScene = "GameScene";

    // Start is called before the first frame update
    void Start()
    {
       network = new NetworkManager();
        
       // Setup start game button
       startGame.onClick.AddListener(OnClickStartGame);
    }

    // Update is called once per frame
    void Update()
    {
      SetText();
    }

    void SetText() {
      playerList.text = network.GetPlayers().Count.ToString();
    }

    void OnClickStartGame() {
      network.ChangeScene(gameScene);
    }
}
