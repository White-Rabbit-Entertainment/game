using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public enum Vote {
  For,
  Against,
}

public class VotingManager : MonoBehaviour {
  public GameObject setVoteUI;
  public GameObject voteInProgress;
  public TextMeshProUGUI votingUIText;
  public PlayersUI playersUI;
  public GameSceneManager gameSceneManager;

  public TextMeshProUGUI votesFor;
  public TextMeshProUGUI votesAgainst;
  public TextMeshProUGUI voteTimeRemaining;
  public TextMeshProUGUI voteTitle;
  public GameObject currentVoteUI;

  public GameObject votingOutcomeUI;
  public GameObject voteUnsuccess;
  public GameObject voteTopRightUI;
  public TextMeshProUGUI voteResult;
  public TextMeshProUGUI voteUnsuccessful;
  
  bool hasVoted = false;
  bool voteStarted = false;
  List<PlayableCharacter> playersVotingFor;
  List<PlayableCharacter> playersVotingAgainst;
  PlayableCharacter suspectedPlayer;
  PlayableCharacter voteLeader;

  public void Update() {
    // Check if the vote has run out of time, if so end the vote
    if (voteStarted) {
      voteTimeRemaining.text = $"{(int)Timer.VoteTimer.TimeRemaining()}s";
      if (Timer.VoteTimer.IsComplete()) {
        EndVote();
      }
    }
    if (voteStarted && !hasVoted && NetworkManager.instance.GetMe() != suspectedPlayer) {
      if (Input.GetKeyDown(KeyCode.Y)) {
        SubmitVote(Vote.For);
      } else if (Input.GetKeyDown(KeyCode.N)) {
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
    yield return new WaitForSeconds(4);
    voteInProgress.SetActive(false);
  }

  [PunRPC]
  public void StartVote(int suspectedPlayerId, int voteLeaderId) {
    playersVotingFor = new List<PlayableCharacter>();
    playersVotingAgainst = new List<PlayableCharacter>();
    suspectedPlayer = PhotonView.Find(suspectedPlayerId).GetComponent<PlayableCharacter>();
    playersUI.SetSuspectedPlayer(suspectedPlayer);
    voteLeader = PhotonView.Find(voteLeaderId).GetComponent<PlayableCharacter>();
    voteStarted = true;

    hasVoted = false;
    voteTopRightUI.SetActive(true);
    
    currentVoteUI.SetActive(true);
    votesFor.text = $"For: 0";
    votesAgainst.text = $"Against: 0";

    bool voteIsOnYou = NetworkManager.instance.GetMe() == suspectedPlayer;
    voteTitle.text = voteIsOnYou ? "You are being voted on." : $"{suspectedPlayer.Owner.NickName} is being voted on.";
    setVoteUI.SetActive(true);
    votingUIText.text = $"Is {suspectedPlayer.Owner.NickName} the traitor?";
  } 

  public void EndVote() {
    setVoteUI.SetActive(false);
    currentVoteUI.SetActive(false);
    voteTopRightUI.SetActive(false);
    playersUI.ClearSuspectedPlayer(suspectedPlayer);
    foreach (PlayableCharacter character in FindObjectsOfType<PlayableCharacter>()) {
      playersUI.ClearVote(character);
    }
    voteStarted = false;
    if (playersVotingFor.Count > playersVotingAgainst.Count) {
      if (suspectedPlayer.IsMe()) {
        suspectedPlayer.Kill();
        if (NetworkManager.instance.NoLoyalsRemaining()) {
          gameSceneManager.EndGame(Team.Traitor);
        }
        if (NetworkManager.instance.NoTraitorsRemaining()) {
          gameSceneManager.EndGame(Team.Loyal);
        }
      }
      // Show UI to say someone was voted off
      ShowVotingOutCome(suspectedPlayer.Owner.NickName);
      StartCoroutine(ShowOutcomeInProgress());
    } else {
      // Show UI to say vote was unsuccessful
      ShowVotingUnsuccess(suspectedPlayer.Owner.NickName);
      StartCoroutine(ShowUnsuccessInProgress());
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
    
  public void ShowVotingOutCome(string name) {
        // Show some UI to say the vote outcome for everyone

        voteResult.text = name + "be voted";
   }

    public void ShowVotingUnsuccess(string name)
    {
        voteUnsuccessful.text = "The vote for " + name + " was unsuccessful";
    }

    IEnumerator ShowOutcomeInProgress()
    {
        votingOutcomeUI.SetActive(true);
        yield return new WaitForSeconds(2);
        votingOutcomeUI.SetActive(false);
    }

    IEnumerator ShowUnsuccessInProgress()
    {
        voteUnsuccess.SetActive(true);
        yield return new WaitForSeconds(2);
        voteUnsuccess.SetActive(false);
    }
}

