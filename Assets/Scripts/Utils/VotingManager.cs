using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public enum Vote {
  For,
  Against,
}

public class VotingManager : MonoBehaviour {
  public GameObject setVoteUI;
  public GameObject voteInProgress;
  public Text votingUIText;
  public PlayersUI playersUI;
  public GameSceneManager gameSceneManager;

  public Text votesFor;
  public Text votesAgainst;
  public Text voteTimeRemaining;
  public Text voteTitle;
  public GameObject currentVoteUI;

  bool hasVoted = false;
  bool voteStarted = false;
  List<PlayableCharacter> playersVotingFor;
  List<PlayableCharacter> playersVotingAgainst;
  PlayableCharacter suspectedPlayer;
  PlayableCharacter voteLeader;

  public void Update() {
    // Check if the vote has run out of time, if so end the vote
    if (voteStarted) {
      if (Timer.VoteTimer.IsComplete()) {
        voteTimeRemaining.text = $"{(int)Timer.VoteTimer.TimeRemaining()}s remaining.";
        EndVote();
      }
    }
    if (voteStarted && !hasVoted && NetworkManager.instance.GetMe() != suspectedPlayer) {
      if (Input.GetKeyDown(KeyCode.K)) {
        SubmitVote(Vote.For);
      } else if (Input.GetKeyDown(KeyCode.L)) {
        SubmitVote(Vote.Against);
      }
    }
  }      

  public void InitVote(int suspectedPlayerId, int voteLeaderId) {
    if (!voteStarted) {
      Timer.VoteTimer.Start(30);
      GetComponent<PhotonView>().RPC("StartVote", RpcTarget.All, suspectedPlayerId, voteLeaderId);
    } else {
      StartCoroutine(ShowVoteInProgress());
    }
  }

  IEnumerator ShowVoteInProgress() {
    voteInProgress.SetActive(true);
    yield return new WaitForSeconds(2);
    voteInProgress.SetActive(false);
  }

  [PunRPC]
  public void StartVote(int suspectedPlayerId, int voteLeaderId) {
    playersVotingFor = new List<PlayableCharacter>();
    playersVotingAgainst = new List<PlayableCharacter>();
    suspectedPlayer = PhotonView.Find(suspectedPlayerId).GetComponent<PlayableCharacter>();
    voteLeader = PhotonView.Find(voteLeaderId).GetComponent<PlayableCharacter>();
    voteStarted = true;
    hasVoted = false;
    currentVoteUI.SetActive(true);
    bool voteIsOnYou = NetworkManager.instance.GetMe() != suspectedPlayer;
    voteTitle.text = voteIsOnYou ? "You are being voted on." : "{suspectedPlayer.NickName is being voted on.}";
    if (voteIsOnYou) {
      setVoteUI.SetActive(true);
      votingUIText.text = $"Is {suspectedPlayer.Owner.NickName} the traitor?";
    }
  } 

  public void EndVote() {
    Timer.VoteTimer.End();
    setVoteUI.SetActive(false);
    currentVoteUI.SetActive(false);
    foreach (PlayableCharacter character in playersVotingFor.Concat(playersVotingAgainst)) {
      playersUI.ClearVote(character);
    }
    voteStarted = false;
    if (playersVotingFor.Count > playersVotingAgainst.Count) {
      if (suspectedPlayer.IsMe()) {
        suspectedPlayer.Kill();
        if (NetworkManager.instance.NoLoyalsRemaining()) {
          // gameSceneManager.EndGame(Team.Traitor);
        }
        if (NetworkManager.instance.NoTraitorsRemaining()) {
          // gameSceneManager.EndGame(Team.Loyal);
        }
      }
      Debug.Log("The player has been voted off");
    } else {
      Debug.Log("The player survived the vote");
    }
  }

  [PunRPC]
  public void SetVote(Vote vote, int votingPlayerId) {
    int numberOfVotingPlayers = NetworkManager.instance.GetPlayers().Count - 1;
    PlayableCharacter votingPlayer = PhotonView.Find(votingPlayerId).GetComponent<PlayableCharacter>();
    if (vote == Vote.For) {
      playersVotingFor.Add(votingPlayer);
    }
    if (vote == Vote.Against) {
      playersVotingAgainst.Add(votingPlayer);
    }
    playersUI.SetPlayerVote(vote, votingPlayer);

    votesFor.text = $"For: {playersVotingFor.Count}";
    votesAgainst.text = $"Against: {playersVotingAgainst.Count}";

    if (playersVotingFor.Count > numberOfVotingPlayers/2 || playersVotingAgainst.Count >= numberOfVotingPlayers/2) {
      EndVote();
    }
  }
  
  public void SubmitVote(Vote vote) {
    GetComponent<PhotonView>().RPC("SetVote", RpcTarget.All, vote, NetworkManager.instance.GetMe().GetComponent<PhotonView>().ViewID);
    hasVoted = true;
    setVoteUI.SetActive(false);
  }
}
