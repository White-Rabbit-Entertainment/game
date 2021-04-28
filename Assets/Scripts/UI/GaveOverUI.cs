using System.Collections;
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
        GetComponent<Image>().color = winningTeam == Team.Traitor ? gameSceneManager.traitorColor : gameSceneManager.loyalColor; 
        continueButton.onClick.AddListener(gameSceneManager.GoToLobby);

        winningTeamText.text = "${Team} have won!";
        foreach (string name in NetworkManager.traitorNames) {
            PlayerTile tile = Instantiate(playerNamePrefab, winnersGrid).GetComponent<PlayerTile>();
        }
    }
}
