using System;
using UnityEngine;
using Photon.Pun;

public class Votable : Interactable {

  public override void PrimaryInteraction(Character voteLeader) {
    GameObject votingManager = GameObject.Find("/VotingManager");
    votingManager.GetComponent<PhotonView>().RPC("StartVote", RpcTarget.All, view.ViewID, voteLeader.GetComponent<PhotonView>().ViewID);
  }
}
    
