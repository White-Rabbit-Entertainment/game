using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Pun;

public class LoadingScreen : MonoBehaviour {

  public GameObject loadingScreen;
  public PlayersUI playersUI;
  public Text text;
  public Button closeButton;

  [TextArea(15,20)]
  private string traitorDescription = "crew's aim is to stop the Loyal crew to complete their tasks whilst not being found out.";
  private string loyalDescription = "crew's aim is to complete your tasks in a given time, if you do so they win the game. All crew members will also have the opportunity to vote a player off the ship if they believe them to be a traitor.";

  private Dictionary<Team, string> descriptions;

  void Start() {
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
    Team team = NetworkManager.instance.GetLocalPlayerProperty<Team>("Team");

    descriptions = new Dictionary<Team, string>();
    descriptions.Add(Team.Loyal, loyalDescription);
    descriptions.Add(Team.Traitor, traitorDescription);

    text.text = $"Welcome to SS White Rabbit, {PhotonNetwork.LocalPlayer.NickName}! You are part of the {team} crew. The {team} {descriptions[team]}";

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
