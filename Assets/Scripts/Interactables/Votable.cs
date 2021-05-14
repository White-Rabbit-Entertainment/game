using System;
using UnityEngine;
using Photon.Pun;

public class Votable : Interactable {

  public VotingManager votingManager;

  public override void PrimaryInteraction(Character voteLeader) {
    if (votingManager == null) {
      votingManager = GameObject.Find("/VotingManager").GetComponent<VotingManager>();
    }
    votingManager.InitVote(View.ViewID, voteLeader.GetComponent<PhotonView>().ViewID);
  }
}
