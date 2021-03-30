using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;

public class LoadingScreen : MonoBehaviour {

  public GameObject loadingScreen;
  public PlayersUI playersUI;
  // public Text text;
  // public TMP_Text text;
  public TextMeshProUGUI titleText;
  public TextMeshProUGUI text;
  public Button closeButton;
  public TeleType teleType;
  public GameObject offScreenIndicator; 

  [TextArea(15,20)]
  private string traitorTitle = "You are a Traitor!";
  private string loyalTitle = "You are a Loyal crewmate!";
  private string traitorDescription = "You are secretely a crewmate of SS White Rabbit's rival ship, SS White Bear. Your mission is to disrupt SS White Rabbit as much as possible by breaking and sabotaging the ship. Be careful and do not get caught, or you fail your mission.";
  private string loyalDescription = "Your ship has been infiltrated and damaged. It needs mending before it sinks. Your mission is to repair the ship before time runs out. Be on the watch out for any crew members acting suspisciously, and vote to throw them off if you beleive them to be a traitor.";

  private Dictionary<Team, string> descriptions;
    private Dictionary<Team, string> titles;

  void Start() {
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
    Team team = NetworkManager.instance.GetLocalPlayerProperty<Team>("Team");

    descriptions = new Dictionary<Team, string>();
    descriptions.Add(Team.Loyal, loyalDescription);
    descriptions.Add(Team.Traitor, traitorDescription);

    titles = new Dictionary<Team, string>();
    titles.Add(Team.Loyal, loyalTitle);
    titles.Add(Team.Traitor, traitorTitle);

    titleText.text = $"{titles[team]}";
    text.text = $"Welcome to SS White Rabbit, {PhotonNetwork.LocalPlayer.NickName}! {descriptions[team]}";
    StartCoroutine(teleType.RevealCharacters());

    closeButton.onClick.AddListener(()=>CloseMenu());
  }

  public void EnableButton() {
    closeButton.interactable = true;
  }

  void CloseMenu() {
    playersUI.Init();
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
    offScreenIndicator.SetActive(true);
    Destroy(loadingScreen);
    Destroy(this);
  }
}
