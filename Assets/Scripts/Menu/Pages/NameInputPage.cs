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
    [SerializeField] private GameObject invalidNameText;
    [SerializeField] private GameObject tooLongNameText;
    
    string pattern = @"^[a-zA-Z0-9]+$";
    Regex inputChecker;

    void OnEnable() {
        base.OnEnable();
        inputChecker = new Regex(pattern);
        playerNameInput.Select();
        playerNameInput.ActivateInputField(); 
        startButton.onClick.AddListener(StartButton);
        invalidNameText.SetActive(false);
        tooLongNameText.SetActive(false);
    }

    public override void OnConnectedToMaster() {
        startButton.interactable = true;
    }

    void StartButton() {
        if (playerNameInput.text.Length > 8) {
            tooLongNameText.SetActive(true);
        } else if (inputChecker.IsMatch(playerNameInput.text)) {
            PhotonNetwork.LocalPlayer.NickName = playerNameInput.text;
            chatManager.Init();
            joinRoomPage.Open();
        } else {
            tooLongNameText.SetActive(false);
            invalidNameText.SetActive(true);
        }    
    }
}
