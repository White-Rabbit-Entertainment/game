using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;

public enum Vote {
  Skip,
  For,
  Against,
}

public class VotingManager : MonoBehaviour {
  public GameObject votingUI;
  public Text votingUIText;
  public PlayersUI playersUI;

  bool voteStarted = false;
  List<PlayableCharacter> playersVotingFor;
  List<PlayableCharacter> playersVotingAgainst;
  List<PlayableCharacter> playersVotingSkip;
  PlayableCharacter suspectedPlayer;
  PlayableCharacter voteLeader;

  [PunRPC]
  public void StartVote(int suspectedPlayerId, int voteLeaderId) {
    playersVotingFor = new List<PlayableCharacter>();
    playersVotingAgainst = new List<PlayableCharacter>();
    playersVotingSkip = new List<PlayableCharacter>();
    suspectedPlayer = PhotonView.Find(suspectedPlayerId).GetComponent<PlayableCharacter>();
    voteLeader = PhotonView.Find(voteLeaderId).GetComponent<PlayableCharacter>();
    voteStarted = true;
    votingUI.SetActive(true);
    votingUIText.text = $"Is {suspectedPlayer.Owner.NickName} the traitor?";
  } 

  [PunRPC]
  public void SetVote(Vote vote, int votingPlayerId) {
    PlayableCharacter votingPlayer = PhotonView.Find(votingPlayerId).GetComponent<PlayableCharacter>();
    if (vote == Vote.For) {
      playersVotingFor.Add(votingPlayer);
    }
    if (vote == Vote.Against) {
      playersVotingAgainst.Add(votingPlayer);
    }
    if (vote == Vote.Skip) {
      playersVotingSkip.Add(votingPlayer);
    }
    playersUI.SetPlayerVote(vote, votingPlayer);
    if (playersVotingFor.Count + playersVotingAgainst.Count + playersVotingSkip.Count == NetworkManager.instance.GetPlayers().Count) {
      votingUI.SetActive(false);
    }
  }
  
  public void SubmitVote(Vote vote) {
    GetComponent<PhotonView>().RPC("SetVote", RpcTarget.All, vote, NetworkManager.instance.GetMe().GetComponent<PhotonView>());
  }
}
