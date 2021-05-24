using System;
using UnityEngine;
using Photon.Pun;

// Extends Interactable to allow an item to be voted on. This is used on
// players. When a vote is started it is managed by the VotingManager.
public class Votable : Interactable {

  public VotingManager votingManager;

  // Request the start of a vote
  public override void PrimaryInteraction(Character voteLeader) {
    if (votingManager == null) {
      votingManager = GameObject.Find("/VotingManager").GetComponent<VotingManager>();
    }
    votingManager.InitVote(View.ViewID, voteLeader.GetComponent<PhotonView>().ViewID);
  }
}
