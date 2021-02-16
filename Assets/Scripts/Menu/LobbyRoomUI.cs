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
  
    private Hashtable props;

    void Start() {
      toggleReadyButton.onClick.AddListener(()=>toggleReady());
    }

    void Update() {
      SetText();

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

    void OnJoin(string team) {
      NetworkManager.instance.ChangeScene("GameScene");
      if (team == "Seeker") {
        PhotonNetwork.Instantiate(seekerPrefab.name, new Vector3(1,2,-10), Quaternion.identity);
      } else if (team == "Robber") {
        PhotonNetwork.Instantiate(robberPrefab.name, new Vector3(1,2,-10), Quaternion.identity);
      }
      NetworkManager.instance.SetLocalPlayerProperty("Team", team);
      GameManager.instance.OnStartGame();
    }
}
