using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
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
  [SerializeField] private TimerManager timerManager;

  public TextMeshProUGUI votesFor;
  public TextMeshProUGUI votesAgainst;
  public TextMeshProUGUI voteTimeRemaining;
  public GameObject currentVoteUI;

  public GameObject votingOutcomeUI;
  public GameObject voteUnsuccess;
  public GameObject voteTopRightUI;
  public TextMeshProUGUI voteResult;
  public TextMeshProUGUI voteUnsuccessful;
  public TextMeshProUGUI helperText;
  
  bool hasVoted = false;
  bool voteStarted = false;
  List<PlayableCharacter> playersVotingFor;
  List<PlayableCharacter> playersVotingAgainst;
  PlayableCharacter suspectedPlayer;
  PlayableCharacter voteLeader;

  public void Update() {
    // Check if the vote has run out of time, if so end the vote
    if (voteStarted) {
      voteTimeRemaining.text = $"{(int)Timer.voteTimer.TimeRemaining()}s";
      if (Timer.voteTimer.IsComplete()) {
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

  [PunRPC]
  /// <summary> When a vote is started, the voting player makes a request to
  /// the master client to start the vote. This ensures only one vote can take
  /// place at a time. </summary>
  public void RequestVote(int suspectedPlayerId, int voteLeaderId) {
    if (!voteStarted && !Timer.sabotageTimer.IsStarted()) {
      voteStarted = true;
      timerManager.StartTimer(Timer.voteTimer);
      GetComponent<PhotonView>().RPC("StartVote", RpcTarget.All, suspectedPlayerId, voteLeaderId);
    } else {
      // Find the player which called the vote
      Player callingVotePlayer = PhotonView.Find(voteLeaderId).Owner;
      StartCoroutine(ShowVoteInProgress());
    }
  }

  [PunRPC]
  /// <summary> When a vote cannot be started because a vote is in progress it
  /// can be rejected by the master client with this funciton. This is called
  /// on the vote caller. </summary>
  public void RejectVote() {
      StartCoroutine(ShowVoteInProgress());
  }


  /// <summary> When a vote is started, we fist check if we think there is
  /// already one in progress. If so we cannot start a vote. If there is not
  /// vote in progress we then request the master client to start a vote. The
  /// master client will not allow 2 votes to be started so this request could
  /// be rejected. If rejected RejectVote will be called on this machine.
  /// </summary>
  public void InitVote(int suspectedPlayerId, int voteLeaderId) {
    if (!voteStarted && !Timer.sabotageTimer.IsStarted()) {
      GetComponent<PhotonView>().RPC("RequestVote", RpcTarget.MasterClient, suspectedPlayerId, voteLeaderId);
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
    votingUIText.text = voteIsOnYou ? "You are being voted on!": $"Is {suspectedPlayer.Owner.NickName} the traitor?";
    helperText.text = voteIsOnYou ? "Convince everyone you're not the traitor" : "Press 'Y' for yes, 'N' for no.";
    setVoteUI.SetActive(true);
  } 

  public void EndVote() {
    setVoteUI.SetActive(false);
    currentVoteUI.SetActive(false);
    voteTopRightUI.SetActive(false);
    Timer.voteTimer.End();
    playersUI.ClearSuspectedPlayer(suspectedPlayer);
    foreach (PlayableCharacter character in FindObjectsOfType<PlayableCharacter>()) {
      playersUI.ClearVote(character);
    }
    voteStarted = false;
    if (playersVotingFor.Count > playersVotingAgainst.Count) {
      if (suspectedPlayer.IsMe()) {
        suspectedPlayer.Kill();
        if (NetworkManager.instance.NumberOfTeamRemaining(Team.Loyal) == NetworkManager.instance.NumberOfTeamRemaining(Team.Traitor)) {
          gameSceneManager.EndGame(Team.Traitor);
        }
        else if (NetworkManager.instance.NoTraitorsRemaining()) {
          gameSceneManager.EndGame(Team.Loyal);
        }
        else if (NetworkManager.instance.NoLoyalsRemaining()) {
          gameSceneManager.EndGame(Team.Traitor);
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
   
    string voteText = vote == Vote.For ? "yes" : "no";
    helperText.text = $"You voted {voteText}";
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

