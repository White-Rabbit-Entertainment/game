using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;

public class LoadingScreen : MonoBehaviour {

  public PlayersUI playersUI;
  public TaskCompletionUI taskCompletionUI;
  public Button closeButton;
  public ContextTaskUI contextTaskUI;

  void Start() {
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
    closeButton.onClick.AddListener(()=>CloseLoadingScreen());
  }

  public void EnableButton() {
    closeButton.interactable = true;
  }

  void CloseLoadingScreen() {
    playersUI.Init();
    contextTaskUI.Init();
    taskCompletionUI.UpdateBar();
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
    Destroy(gameObject);
    Destroy(this);
  }
}
