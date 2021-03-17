using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class PlayersUI : MonoBehaviourPun {

  public GameObject livingPlayerTile;
  public GameObject deadPlayerTile;
  public GameObject playerList;

  void Update() {
    EmptyList();

    foreach (Player player in NetworkManager.instance.GetPlayers()) {
      if (NetworkManager.instance.PlayerPropertyIs<Team>("Team", Team.Ghost, player)) {
        AddDeadPlayer(player);
      } else {
        AddLivingPlayer(player);
      }
    }
  }

  void EmptyList() {
    foreach (Transform child in playerList.transform) {
      Destroy(child.gameObject);
    }
  }

  void AddLivingPlayer(Player player) {
    GameObject item = Instantiate(livingPlayerTile, playerList.transform);
    TMP_Text text = item.GetComponentInChildren<TMP_Text>();
    text.text = player.NickName;
  }

  void AddDeadPlayer(Player player) {
    GameObject item = Instantiate(deadPlayerTile, playerList.transform);
    TMP_Text text = item.GetComponentInChildren<TMP_Text>();
    text.text = player.NickName;
  }
}
