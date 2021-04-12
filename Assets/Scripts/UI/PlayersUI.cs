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
  public GameObject suspectTile;
    
  public void Init() {
    foreach (PlayableCharacter player in FindObjectsOfType<PlayableCharacter>()) {
      AddPlayerTile(player);
    }
  }

  void AddPlayerTile(PlayableCharacter player) {
    GameObject item = Instantiate(playerTile, playerList.transform);
    player.playerTile = item;
    player.playersUI = this;

    // Set colour of tile to match player role
    item.GetComponent<Image>().color = player.roleInfo.colour; 

    // Set text to name
    TextMeshProUGUI playerName = item.GetComponentInChildren<TextMeshProUGUI>();
    // text.text = player.Owner.NickName + " (" + player.roleInfo.name + ")";
    playerName.text = player.Owner.NickName;
   
    // If the player is dead cross them out
    if (player is Ghost) {
      SetToDead(player);
    }
  }

  void RedoPlayerTiles(PlayableCharacter suspectedPlayer){
    foreach (PlayableCharacter player in FindObjectsOfType<PlayableCharacter>()) {
      if (player != suspectedPlayer){
        AddPlayerTile(player);
      }
    }
  }

  public void SetPlayerVote(Vote vote, PlayableCharacter player) {
    GameObject item = player.playerTile;
    if (vote == Vote.For) {
      item.transform.Find("VoteFor").gameObject.SetActive(true);
    } else if (vote == Vote.Against) {
      item.transform.Find("VoteAgainst").gameObject.SetActive(true);
    }
  }

  public void SetSuspectedPlayer(PlayableCharacter suspectedPlayer){
    suspectedPlayer.playerTile.transform.Find("votingMarkAppear").gameObject.SetActive(true);
    suspectedPlayer.playerTile.transform.position = suspectTile.transform.position;
    suspectedPlayer.transform.SetParent(playerList.transform,false);
    suspectedPlayer.transform.SetParent(suspectTile.transform,true);
    // playerList.Remove(suspectedPlayer);
    // foreach (PlayableCharacter player in FindObjectsOfType<PlayableCharacter>()) {
    //   if (player != suspectedPlayer){
    //     Destroy(player.playerTile);
    //   }
    // }
    // RedoPlayerTiles(suspectedPlayer);
  }

  public void ClearSuspectedPlayer(PlayableCharacter suspectedPlayer){
    // foreach (PlayableCharacter player in FindObjectsOfType<PlayableCharacter>()) {
    //   Destroy(player.playerTile);
    // }
    // Init();
    suspectedPlayer.transform.SetParent(suspectTile.transform,false);
    suspectedPlayer.transform.SetParent(playerList.transform,true);
    
  }

  public void ClearVote(PlayableCharacter character) {
    character.playerTile.transform.Find("VoteFor").gameObject.SetActive(false);
    character.playerTile.transform.Find("VoteAgainst").gameObject.SetActive(false);
    character.playerTile.transform.Find("votingMarkAppear").gameObject.SetActive(false);
    suspectTile.SetActive(false);
  }

  public void SetToDead(PlayableCharacter character) {
      Transform cross = character.playerTile.transform.Find("Cross");
      cross.gameObject.SetActive(true);
  }
}
