using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinRoomUI: MonoBehaviour {

    public InputField roomInput; 
    public Button createRoomButton; 
    public Button joinRoomButton; 

    private NetworkManager networkManager;
    // Start is called before the first frame update
    void Start() {
      networkManager = new NetworkManager();

      // Setup listenors 
      createRoomButton.onClick.AddListener(OnClickCreateRoom);
      joinRoomButton.onClick.AddListener(OnClickJoinRoom);
    }

    // Update is called once per frame
    void OnClickCreateRoom()
    {
      networkManager.CreateRoom(roomInput.text);
    }
    
    void OnClickJoinRoom()
    {
      networkManager.JoinRoom(roomInput.text);
    }
}
