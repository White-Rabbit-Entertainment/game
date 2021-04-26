using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Text.RegularExpressions;

public class NameInputPage : MenuPage {
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private Button startButton;
    [SerializeField] private JoinRoomPage joinRoomPage;
    [SerializeField] private ChatManager chatManager;
    
    string pattern = @"^[a-zA-Z0-9]+$";
    Regex inputChecker;

    void OnEnable() {
        base.OnEnable();
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
            chatManager.Init();
            joinRoomPage.Open();
        } else {
            Debug.Log("Please enter a valid name");
        }    
    }
}
