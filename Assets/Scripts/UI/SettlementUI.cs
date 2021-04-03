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

    // Update is called once per frame
    public void OnGameOver(Team team) {
        nextButtonTraitor.onClick.AddListener(GoToLobby);
        nextButtonLoyal.onClick.AddListener(GoToLobby);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        traitorName.text = NetworkManager.traitorNames.ToString();
        if (team == Team.Traitor) {
            TraitorsWonUI.SetActive(true);
            TraitorInfoUI.SetActive(true);
        } else {
            LoyalsWonUI.SetActive(true);
            TraitorInfoUI.SetActive(true);
        }
    }

    void GoToLobby() {
        NetworkManager.instance.ChangeScene("LobbyScene");
    }
}
