﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using TMPro;


public class GameOverUI : MonoBehaviour { 

    [SerializeField] private GameSceneManager gameSceneManager;

    [SerializeField] private TMP_Text winningTeamText;
    [SerializeField] private Transform winnersGrid;
    [SerializeField] private Button continueButton;
    
    [SerializeField] private GameObject playerNamePrefab;

    // Update is called once per frame
    public void OnGameOver(Team winningTeam) {

        Debug.Log("Ongame over happening");
        GetComponent<Image>().color = winningTeam == Team.Traitor ? gameSceneManager.traitorColor : gameSceneManager.loyalColor; 
        continueButton.onClick.AddListener(gameSceneManager.GoToLobby);

        int numberOfTraitors = 0;
        foreach (PlayableCharacter player in FindObjectsOfType<PlayableCharacter>()) {
            if (player.startingTeam == Team.Traitor) {
                numberOfTraitors++;
            }

            if (player.startingTeam == winningTeam) {
                PlayerTile tile = Instantiate(playerNamePrefab, winnersGrid).GetComponent<PlayerTile>();
                tile.Init(player);
            }
        }
        
        // Set winning text
        if (winningTeam == Team.Traitor) {
            if (numberOfTraitors == 1) {
                winningTeamText.text = "The traitor has won!";
            } else {
                winningTeamText.text = "The traitors have won!";
            }
        } else {
            winningTeamText.text = "The loyals have won!";
        }
        
        gameObject.SetActive(true);
    }
}