using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyRoomUI : MonoBehaviour
{

    public Text playerList;
    private NetworkManager network;
    // Start is called before the first frame update
    void Start()
    {
       network = new NetworkManager();
    }

    // Update is called once per frame
    void Update()
    {
      SetText();
    }

    void SetText() {
      playerList.text = network.GetPlayers().ToString();
    }
}
