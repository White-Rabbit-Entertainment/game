using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class PlayersUI : MonoBehaviourPun {

  public GameObject playerTile;
  public GameObject suspectTile;
    
  public void Init() {
    foreach (PlayableCharacter player in FindObjectsOfType<PlayableCharacter>()) {
      AddPlayerTile(player);
    }
  }

  void AddPlayerTile(PlayableCharacter player) {
    PlayerTile tile = Instantiate(playerTile, transform).GetComponent<PlayerTile>();
    player.playerTile = tile;
    player.playersUI = this;
    tile.Init(player);
  }

  void RedoPlayerTiles(PlayableCharacter suspectedPlayer){
    foreach (PlayableCharacter player in FindObjectsOfType<PlayableCharacter>()) {
      if (player != suspectedPlayer){
        AddPlayerTile(player);
      }
    }
  }

  public void SetPlayerVote(Vote vote, PlayableCharacter player) {
    PlayerTile tile = player.playerTile;
    if (vote == Vote.For) {
      tile.EnableVotingFor();
    } else if (vote == Vote.Against) {
      tile.EnableVotingAgainst();
    }
  }

  public void SetSuspectedPlayer(PlayableCharacter suspectedPlayer){
    // suspectedPlayer.playerTile.EnableVotingMark();
    suspectedPlayer.playerTile.transform.position = suspectTile.transform.position;
    suspectedPlayer.playerTile.transform.SetParent(suspectTile.transform,true);
  }

  public void ClearSuspectedPlayer(PlayableCharacter suspectedPlayer){
    suspectedPlayer.playerTile.transform.SetParent(transform, true);
  }

  public void ClearVote(PlayableCharacter character) {
    character.playerTile.Clear();
  }

  public void SetToDead(PlayableCharacter character) {
      character.playerTile.EnableCross();
  }
}
