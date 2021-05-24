using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;

// Possible vote values
public enum Vote {
  For,
  Against,
}

public class VotingManager : MonoBehaviour {

  // UI 
  public GameObject setVoteUI;
  public GameObject voteInProgress;
  public TextMeshProUGUI votingUIText;
  public PlayersUI playersUI;
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

  // Managers
  public GameSceneManager gameSceneManager;
  [SerializeField] private TimerManager timerManager;

  // Music
  [SerializeField] private AudioSource votingMusic;
  
  bool hasVoted = false;
  bool voteStarted = false;

  // The list of player which have submitted for votes
  List<PlayableCharacter> playersVotingFor;
  // The list of player which have submitted against votes
  List<PlayableCharacter> playersVotingAgainst;

  // The player that is suspected
  PlayableCharacter suspectedPlayer;

  // The player that called the vote
  PlayableCharacter voteLeader;

  public void Update() {
    // Check if the vote has run out of time, if so end the vote
    if (voteStarted) {
      voteTimeRemaining.text = $"{(int)Timer.voteTimer.TimeRemaining()}s";
      if (Timer.voteTimer.IsComplete()) {
        EndVote();
      }
    }

    // Listen for key input for voting 
    if (voteStarted && !hasVoted && NetworkManager.instance.GetMe() != suspectedPlayer) {
      // If press "y" vote for
      if (Input.GetKeyDown(KeyCode.Y)) {
        SubmitVote(Vote.For);

      // If press "n" vote for
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

    // If vote cannot be started then dont request and just show UI
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

  // UI for when a vote cannot be started become one is already happening 
  IEnumerator ShowVoteInProgress() {
    voteInProgress.SetActive(true);
    yield return new WaitForSeconds(4);
    voteInProgress.SetActive(false);
  }

  // Start a vote
  [PunRPC]
  public void StartVote(int suspectedPlayerId, int voteLeaderId) {
    
    voteStarted = true;
    hasVoted = false;

    // Empty list of whos voted for what 
    playersVotingFor = new List<PlayableCharacter>();
    playersVotingAgainst = new List<PlayableCharacter>();

    // Set the voting players
    suspectedPlayer = PhotonView.Find(suspectedPlayerId).GetComponent<PlayableCharacter>();
    playersUI.SetSuspectedPlayer(suspectedPlayer);
    voteLeader = PhotonView.Find(voteLeaderId).GetComponent<PlayableCharacter>();

    // UI
    voteTopRightUI.SetActive(true);
    currentVoteUI.SetActive(true);
    votesFor.text = $"For: 0";
    votesAgainst.text = $"Against: 0";
    // Check if the vote is on you
    bool voteIsOnYou = NetworkManager.instance.GetMe() == suspectedPlayer;
    // At set text of UI based on if you are being voted on or not
    votingUIText.text = voteIsOnYou ? "You are being voted on!": $"Is {suspectedPlayer.Owner.NickName} the traitor?";
    helperText.text = voteIsOnYou ? "Convince everyone you're not the traitor" : "Press 'Y' for yes, 'N' for no.";
    setVoteUI.SetActive(true);

    // Start voting music
    votingMusic.Play();
  } 

  public void EndVote() {
    voteStarted = false;

    // UI for ending vote
    setVoteUI.SetActive(false);
    currentVoteUI.SetActive(false);
    voteTopRightUI.SetActive(false);
    playersUI.ClearSuspectedPlayer(suspectedPlayer);

    // End timer
    Timer.voteTimer.End();

    // Update players UI 
    foreach (PlayableCharacter character in FindObjectsOfType<PlayableCharacter>()) {
      playersUI.ClearVote(character);
    }

    // Handle voting off player
    if (playersVotingFor.Count > playersVotingAgainst.Count) {
      if (suspectedPlayer.IsMe()) {
        // Voted off player should be killer
        suspectedPlayer.Kill();
    
        // Check if the game should now end as a result of the vote
        // Same number of traitors left as loyals - Traitors win
        if (NetworkManager.instance.NumberOfTeamRemaining(Team.Loyal) == NetworkManager.instance.NumberOfTeamRemaining(Team.Traitor)) {
          gameSceneManager.EndGame(Team.Traitor);
        }
        // No traitors left - Loyals win
        else if (NetworkManager.instance.NoTraitorsRemaining()) {
          gameSceneManager.EndGame(Team.Loyal);
        }
        // No loayls left - Traitors win
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

    // Stop music
    votingMusic.Stop();
  }

  // Set a players vote for all player
  [PunRPC]
  public void SetVote(Vote vote, int votingPlayerId) {
    // -1 as player being voted on does not vote
    int numberOfVotingPlayers = NetworkManager.instance.GetPlayers().Count - 1;

    // Get the player who is voting
    PlayableCharacter votingPlayer = PhotonView.Find(votingPlayerId).GetComponent<PlayableCharacter>();

    // Add player to correct player list 
    if (vote == Vote.For) {
      playersVotingFor.Add(votingPlayer);
    }
    if (vote == Vote.Against) {
      playersVotingAgainst.Add(votingPlayer);
    }

    // Set UI
    playersUI.SetPlayerVote(vote, votingPlayer);
    votesFor.text = $"For: {playersVotingFor.Count}";
    votesAgainst.text = $"Against: {playersVotingAgainst.Count}";

    // Check if the vote should now be ended
    if (playersVotingFor.Count > numberOfVotingPlayers/2 || playersVotingAgainst.Count >= numberOfVotingPlayers/2) {
      EndVote();
    }
  }
 
  // Submit your vote to all players
  public void SubmitVote(Vote vote) {
    GetComponent<PhotonView>().RPC("SetVote", RpcTarget.All, vote, NetworkManager.instance.GetMe().GetComponent<PhotonView>().ViewID);
    hasVoted = true;
  
    // Update UI
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

