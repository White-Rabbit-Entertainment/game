using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;

public class VoteOutcomeUI : MonoBehaviour
{
    public GameSceneManager gameSceneManager;
    public TaskManager taskManager;
    public VotingManager votingManager;

    public GameObject voteOutcomeUI;
    public GameObject beVotedUI;

    public Text voteResult;
    public Text beVoted;

    public Button confirmButton;

    // Start is called before the first frame update
    void Start()
    {
        confirmButton.onClick.AddListener(OffConfirm);
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkManager.instance.LocalPlayerPropertyIs("Ghost", true))
        {
            beVotedUI.SetActive(true);
        }

        //if (NetworkManager.instance.SetPlayerProperty("Team", Team.Ghost, player))
        //{
        //    voteOutcomeUI.SetActive(true);
        //   //之后要怎么判断?
        //}

    }

    


void OffConfirm()
    {
        Destroy(beVotedUI, 2);
    }

    
}
