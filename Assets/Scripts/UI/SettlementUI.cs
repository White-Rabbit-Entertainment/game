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


    // Start is called before the first frame update
    void Start()
    {
        nextButtonTraitor.onClick.AddListener(OnNextTraitor);
        nextButtonLoyal.onClick.AddListener(OnNextLoyal);
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkManager.instance.NoLoyalsRemaining())
        {
            TraitorsWonUI.SetActive(true);
            

            //gameSceneManager.EndGame(Team.Traitor);
        }

        if (NetworkManager.instance.NoTraitorsRemaining())
        {
            LoyalsWonUI.SetActive(true);

            //gameSceneManager.EndGame(Team.Loyal);
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
