using System;
using UnityEngine;
using Photon.Pun;

public class Votable : Interactable {

  public GameObject votingManager;

  public void Start() {
    base.Start();
    votingManager = GameObject.Find("/VotingManager");
  }

  public override void PrimaryInteraction(Character voteLeader) {
    votingManager.GetComponent<VotingManager>().InitVote(view.ViewID, voteLeader.GetComponent<PhotonView>().ViewID);
  }
}
