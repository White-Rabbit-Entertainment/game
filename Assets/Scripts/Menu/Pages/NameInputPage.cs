using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Text.RegularExpressions;

public class NameInputPage : MenuPage {
    public TMP_InputField playerNameInput;
    public Button startButton;
    public JoinRoomPage joinRoomPage;
    string pattern = @"^[a-zA-Z0-9]+$";
    Regex inputChecker;
    
    void Start() {
        inputChecker = new Regex(pattern);
        playerNameInput.Select();
        playerNameInput.ActivateInputField(); 
        startButton.onClick.AddListener(StartButton);
    }

    public override void OnConnectedToMaster() {
        startButton.interactable = true;
    }
    
    void StartButton() {
        if (inputChecker.IsMatch(playerNameInput.text)) {
            PhotonNetwork.LocalPlayer.NickName = playerNameInput.text;
            joinRoomPage.Open();
        } else {
            Debug.Log("Please enter a valid name");
        }    
    }
}
