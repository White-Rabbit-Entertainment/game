using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;


public class SettlementUI : MonoBehaviour
{
    public GameObject TraitorsWonUI;
    public GameObject LoyalsWonUI;
    public GameObject TraitorInfoUI;

    public Button nextButtonTraitor;
    public Button nextButtonLoyal;

    public GameSceneManager gameSceneManager;
    public TaskManager taskManager;

    public Text traitorName;

   


    // Start is called before the first frame update
    void Start()
    {
        nextButtonTraitor.onClick.AddListener(OnNextTraitor);
        nextButtonLoyal.onClick.AddListener(OnNextLoyal);

        traitorName.text = "FZS";
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkManager.instance.NoLoyalsRemaining())
        {
            TraitorsWonUI.SetActive(true);
            TraitorInfoUI.SetActive(true);
            

            //gameSceneManager.EndGame(Team.Traitor);
        }

        if (NetworkManager.instance.NoTraitorsRemaining())
        {
            LoyalsWonUI.SetActive(true);
            TraitorInfoUI.SetActive(true);

            //gameSceneManager.EndGame(Team.Loyal);
        }

        //if (number of tasks comp) == tasks.count Loyalwon true
        if (taskManager.NumberOfTasksCompleted() == taskManager.tasks.Count)
        {
            //LoyalsWonUI.SetActive(true);
            //TraitorInfoUI.SetActive(true);
        }

        if (Timer.RoundTimer.IsComplete())
        {
            TraitorsWonUI.SetActive(true);
            TraitorInfoUI.SetActive(true);
        }

        


    }

    void OnNextTraitor()
    {
        gameSceneManager.EndGame(Team.Traitor);
    }

    void OnNextLoyal()
    {
        gameSceneManager.EndGame(Team.Loyal);
    }

    
   
    
  

    //public GameSceneManager gameScenManager;


    
}
