using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class PlayersUI : MonoBehaviourPun {

  public GameObject playerTile;
  public GameObject playerList;

  public void Init() {
    foreach (PlayableCharacter player in FindObjectsOfType<PlayableCharacter>()) {
      AddPlayerTile(player);
    }
  }

  void AddPlayerTile(PlayableCharacter player) {
    GameObject item = Instantiate(playerTile, playerList.transform);

    // Set colour of tile to match player role
    item.GetComponent<Image>().color = player.roleInfo.colour; 

    // Set text to name
    TMP_Text text = item.GetComponentInChildren<TMP_Text>();
    text.text = player.Owner.NickName + " (" + player.roleInfo.name + ")";
   
    // If the player is dead cross them out
    if (player is Ghost) {
      Transform cross = transform.FindChild("Cross");
      cross.gameObject.SetActive(true);
    }
  }
}
