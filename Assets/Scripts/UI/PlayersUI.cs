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
    
  private Dictionary<PlayableCharacter, GameObject> playerTiles;

  public void Init() {
    playerTiles = new Dictionary<PlayableCharacter, GameObject>();
    foreach (PlayableCharacter player in FindObjectsOfType<PlayableCharacter>()) {
      AddPlayerTile(player);
    }
  }

  void AddPlayerTile(PlayableCharacter player) {
    GameObject item = Instantiate(playerTile, playerList.transform);
    playerTiles[player] = item;

    // Set colour of tile to match player role
    item.GetComponent<Image>().color = player.roleInfo.colour; 

    // Set text to name
    TMP_Text text = item.GetComponentInChildren<TMP_Text>();
    text.text = player.Owner.NickName + " (" + player.roleInfo.name + ")";
   
    // If the player is dead cross them out
    if (player is Ghost) {
      Transform cross = item.transform.Find("Cross");
      cross.gameObject.SetActive(true);
    }
  }

  public void SetPlayerVote(Vote vote, PlayableCharacter player) {
    GameObject item = playerTiles[player];
    if (vote == Vote.For) {
      item.transform.Find("VoteFor").gameObject.SetActive(true);
    } else if (vote == Vote.Against) {
      item.transform.Find("VoteAgainst").gameObject.SetActive(true);
    } else if (vote == Vote.Skip) {
      item.transform.Find("VoteSkip").gameObject.SetActive(true);
    }
  }

  public void ClearVote() {
    foreach(GameObject item in playerTiles.Values) {
      item.transform.Find("VoteFor").gameObject.SetActive(false);
      item.transform.Find("VoteAgainst").gameObject.SetActive(false);
      item.transform.Find("VoteSkip").gameObject.SetActive(false);
    }
  }
}
