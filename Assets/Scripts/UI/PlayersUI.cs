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

  public void Init() {
    foreach (PlayableCharacter player in FindObjectsOfType<PlayableCharacter>()) {
      AddPlayerTile(player);
    }
  }

  void AddPlayerTile(PlayableCharacter player) {
    GameObject item;
    if (player is Ghost) {
      item = Instantiate(deadPlayerTile, playerList.transform);
    } else {
      item = Instantiate(livingPlayerTile, playerList.transform);
    }
    TMP_Text text = item.GetComponentInChildren<TMP_Text>();
    text.text = player.owner.NickName + " (" + player.roleInfo.name + ")";
  }
}
