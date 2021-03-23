using UnityEngine;
using Photon.Pun;
using System;

public enum Vote {
  Skip,
  For,
  Against,
}

public class VotingManager : MonoBehaviour {
  bool voteStarted = false;
  GameObject votingUI;
  int votesFor;
  int votesAgainst;
  PlayableCharacter suspectedPlayer;
  PlayableCharacter voteLeader;

  public void InitVote(PlayableCharacter suspectedPlayer) {
    votesFor = 0;
    votesAgainst = 0;
    view.RPC("StartVote", RpcTarget.All, suspectedPlayer.view.id, NetworkManager.instance.GetMe().view.id);
  } 

  [PunRPC]
  public void StartVote(int suspectedPlayerId, int voteLeaderId) {
    suspectedPlayer = PhotonView.Find(suspectedPlayerId).GetComponent<PlayableCharacter>();
    voteLeader = PhotonView.Find(voteLeaderId).GetComponent<PlayableCharacter>();
    voteStarted = true;
    votingUI.SetActive(true);
  } 

  [PunRPC]
  public void SetVote(Vote vote) {
    if (vote == Vote.For) {
      votesFor++;
    }
    if (vote == Vote.Against) {
      votesAgainst++;
    }
  }
  
  public void SubmitVote(Vote vote) {
    view.RPC("SetVote", voteLeader.Owner, vote);
    votingUI.SetActive(false);
  }
}
