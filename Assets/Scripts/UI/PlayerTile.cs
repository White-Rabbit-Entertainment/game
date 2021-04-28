using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerTile : MonoBehaviour {
    private GameObject cross;
    private GameObject votingFor;
    private GameObject votingAgainst;
    private GameObject votingMark;

    public void Start() {
        votingFor = transform.Find("VoteFor").gameObject;
        votingAgainst = transform.Find("VoteAgainst").gameObject;
        cross = transform.Find("Cross").gameObject;
        votingMark = transform.Find("votingMarkAppear").gameObject;
    }

    public void Init(PlayableCharacter player) {
        // Set colour of tile to match player role
        GetComponent<Image>().color = player.roleInfo.colour; 

        // Set text to name
        TextMeshProUGUI playerName = GetComponentInChildren<TextMeshProUGUI>();
        playerName.text = player.Owner.NickName;
    
        // If the player is dead cross them out
        if (player is Ghost) {
          EnableCross();
        }
    }

    public void EnableVotingFor(bool enable = true) {
        votingFor.SetActive(enable);
    }
    
    public void EnableVotingAgainst(bool enable = true) {
        votingAgainst.SetActive(enable);
    }
      
    public void EnableCross(bool enable = true) {
        cross.SetActive(enable);
    }
    
    public void EnableVotingMark(bool enable = true) {
        votingMark.SetActive(enable);
    }

    public void Clear() {
        EnableVotingFor(false);
        EnableVotingAgainst(false);
        EnableVotingMark(false);
    }
}
