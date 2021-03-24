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

  bool hasVoted = false;
  bool voteStarted = false;
  List<PlayableCharacter> playersVotingFor;
  List<PlayableCharacter> playersVotingAgainst;
  List<PlayableCharacter> playersVotingSkip;
  PlayableCharacter suspectedPlayer;
  PlayableCharacter voteLeader;

  public void Update() {
    if (voteStarted && !hasVoted) {
      if (Input.GetKeyDown(KeyCode.O)) {
        SubmitVote(Vote.For);
      } else if (Input.GetKeyDown(KeyCode.P)) {
        SubmitVote(Vote.Against);
      } else if (Input.GetKeyDown(KeyCode.L)) {
        SubmitVote(Vote.Skip);
      }
    }
  }      

  [PunRPC]
  public void StartVote(int suspectedPlayerId, int voteLeaderId) {
    playersVotingFor = new List<PlayableCharacter>();
    playersVotingAgainst = new List<PlayableCharacter>();
    playersVotingSkip = new List<PlayableCharacter>();
    suspectedPlayer = PhotonView.Find(suspectedPlayerId).GetComponent<PlayableCharacter>();
    voteLeader = PhotonView.Find(voteLeaderId).GetComponent<PlayableCharacter>();
    voteStarted = true;
    hasVoted = false;
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
      playersUI.ClearVote();
    }
  }
  
  public void SubmitVote(Vote vote) {
    GetComponent<PhotonView>().RPC("SetVote", RpcTarget.All, vote, NetworkManager.instance.GetMe().GetComponent<PhotonView>().ViewID);
    hasVoted = true;
    votingUI.SetActive(false);
  }
}
