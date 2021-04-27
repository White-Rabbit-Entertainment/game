using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using TMPro;


public class SettlementUI : MonoBehaviour
{ 

    public GameSceneManager gameSceneManager;
    public TaskManager taskManager;

    [SerializeField] private TMP_Text winningTeamText;
    [SerializeField] private GameObject winnersGrid;
    [SerializeField] private Button continueButton;
    private Image backgroundImage;

    void Start() {
        backgroundImage = GetComponent<Image>();
    }

    // Update is called once per frame
    public void OnGameOver(Team team) {

        backgroundImage.color = team == Team.Traitor ? gameSceneManager.traitorColor : gameSceneManager.loyalColor; 

        continueButton.onClick.AddListener(GoToLobby);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        winningTeamText.text = NetworkManager.traitorNames.ToString();
    }

    void GoToLobby() {
        NetworkManager.instance.ChangeScene("LobbyScene");
    }
}
