using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class NameInputPage : MenuPage {
    public TMP_InputField playerNameInput;
    public Button startButton;
    public JoinRoomPage joinRoomPage;
    
    void Start() {
        playerNameInput.Select();
        playerNameInput.ActivateInputField(); 
        startButton.onClick.AddListener(StartButton);
    }

    public override void OnConnectedToMaster() {
        startButton.interactable = true;
    }
    
    void StartButton() {
        PhotonNetwork.LocalPlayer.NickName = playerNameInput.text;
        joinRoomPage.Open();
    }
}
