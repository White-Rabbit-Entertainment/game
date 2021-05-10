using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Text.RegularExpressions;
using System.Collections;


public class NameInputPage : MenuPage {
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private Button startButton;
    [SerializeField] private JoinRoomPage joinRoomPage;
    [SerializeField] private ChatManager chatManager;
    [SerializeField] private GameObject invalidNameText;
    [SerializeField] private GameObject tooLongNameText;
    [SerializeField] private GameObject container;


    
    string pattern = @"^[a-zA-Z0-9]+$";
    Regex inputChecker;

    void OnEnable() {
        StartCoroutine(DelayEnable());
    }

    IEnumerator DelayEnable(){
        container.SetActive(false);
        yield return new WaitForSeconds(10f);
        container.SetActive(true);
        if(menuManager.isConnected){
            OnConnectedToMaster();
        }
        base.OnEnable();
        inputChecker = new Regex(pattern);
        playerNameInput.Select();
        playerNameInput.ActivateInputField(); 
        startButton.onClick.AddListener(StartButton);
        invalidNameText.SetActive(false);
        tooLongNameText.SetActive(false);
    }

    // void Start(){
    //     inputChecker = new Regex(pattern);
    //     playerNameInput.Select();
    //     playerNameInput.ActivateInputField(); 
    //     startButton.onClick.AddListener(StartButton);
    //     invalidNameText.SetActive(false);
    //     tooLongNameText.SetActive(false);
    // }
    public override void OnConnectedToMaster() {
        startButton.interactable = true;
    }

    void StartButton() {
        if (playerNameInput.text.Length > 8) {
            playerNameInput.text = "";
            StartCoroutine(SetText(tooLongNameText,1f));
        } else if (inputChecker.IsMatch(playerNameInput.text)) {
            PhotonNetwork.LocalPlayer.NickName = playerNameInput.text;
            chatManager.Init();
            joinRoomPage.Open();
        } else {
            tooLongNameText.SetActive(false);
            playerNameInput.text = "";
            StartCoroutine(SetText(invalidNameText,3f));
        }    
    }

    IEnumerator SetText(GameObject text, float time){
        text.SetActive(true);
        yield return new WaitForSeconds(time);
        text.SetActive(false);
    }
}
