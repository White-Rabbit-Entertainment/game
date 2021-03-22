using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Pun;

public class LoadingScreen : MonoBehaviour {
  
  public GameObject loadingScreen;
  public PlayersUI playersUI; 
  public Text text; 
  public Button closeButton;

  public string capatianDescription;
  public string traitorDescription;
  public string loyalDescription;

  private Dictionary<Team, string> descriptions;

  void Start() {
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
    Team team = NetworkManager.instance.GetLocalPlayerProperty<Team>("Team");

    descriptions = new Dictionary<Team, string>();
    descriptions.Add(Team.Captain, capatianDescription);
    descriptions.Add(Team.NonCaptainLoyal, loyalDescription);
    descriptions.Add(Team.Traitor, traitorDescription);

    text.text = $"Welcome {PhotonNetwork.LocalPlayer.NickName}! You are a {team}. {descriptions[team]}";

    closeButton.onClick.AddListener(()=>CloseMenu());
  }

  public void EnableButton() {
    closeButton.interactable = true;
  }

  void CloseMenu() {
    playersUI.Init();
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
    Destroy(loadingScreen);
    Destroy(this);
  }
}
