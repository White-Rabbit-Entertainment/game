using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class DeathUI : MonoBehaviour {

    [SerializeField] private Button continueButton; 

    void OnEnable() {
        continueButton.onClick.AddListener(Continue);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        NetworkManager.instance.GetMe().Freeze();
    }

    void Continue() {
        gameObject.SetActive(false);
        NetworkManager.instance.GetMe().Unfreeze();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
