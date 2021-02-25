using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyRoomUI : MonoBehaviourPun {
    public Text playerCounter;
    public GameObject playerList;
    public Button toggleReadyButton;
    public GameObject robberPrefab;
    public GameObject seekerPrefab;
    public GameObject readyPlayerItemPrefab;
    public GameObject unreadyPlayerItemPrefab;


    //RoomList
    public GameObject roomNamePrefab;
    public Transform gridLayout;

    private Hashtable props;

    void Start() {
      Cursor.lockState = CursorLockMode.None;
      toggleReadyButton.onClick.AddListener(()=>toggleReady());
    }

    void Update() {
      SetText();
      if (NetworkManager.instance.AllPlayersReady()) {
        GameManager.instance.SetupGame();
        if (NetworkManager.instance.RoomPropertyIs<bool>("GameReady", true)) {
          NetworkManager.instance.SetRoomProperty("GameStarted", true);
          GameManager.instance.StartGame();
          Destroy(this);
        }
      }
    }

    void SetText() {
      foreach (Transform child in playerList.transform) {
        Destroy(child.gameObject);
      }
      foreach (Player player in NetworkManager.instance.GetPlayers()) {
        GameObject playerItemPrefab; 
        if (NetworkManager.instance.PlayerPropertyIs("Ready", true, player)) {
          playerItemPrefab = readyPlayerItemPrefab;
        } else {
          playerItemPrefab = unreadyPlayerItemPrefab;
        }
        GameObject item = Instantiate(playerItemPrefab, transform);
        item.GetComponentInChildren<Text>().text = player.NickName;
        item.transform.SetParent(playerList.transform);
      }
      
      playerCounter.text = NetworkManager.instance.GetPlayers().Count.ToString();
    }

    void toggleReady() {
      if (NetworkManager.instance.LocalPlayerPropertyIs<bool>("Ready", true)) {
        NetworkManager.instance.SetLocalPlayerProperty("Ready", false);
      } else {
        NetworkManager.instance.SetLocalPlayerProperty("Ready", true);
      }
    }


    public class RoomListManager : MonoBehaviourPunCallbacks
    {
        public GameObject roomNamePrefab;
        public Transform gridLayout;

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            for (int i = 0; i < gridLayout.childCount; i++)
            {
                if (gridLayout.GetChild(i).gameObject.GetComponentInChildren<Text>().text == roomList[i].Name)
                {
                    Destroy(gridLayout.GetChild(i).gameObject);

                    if (roomList[i].PlayerCount == 0)
                    {
                        roomList.Remove(roomList[i]);
                    }
                }
            }


            foreach (var room in roomList)
            {
                GameObject newRoom = Instantiate(roomNamePrefab, gridLayout.position, Quaternion.identity);

                newRoom.GetComponentInChildren<Text>().text = room.Name + "(" + room.PlayerCount + ")";
                newRoom.transform.SetParent(gridLayout);
            }
        }
    }
}
