using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class DeathUI : MonoBehaviour {

    [SerializeField] private Button continueButton; 

    void OnEnable() {
        continueButton.onClick.AddListener(Continue);
    }

    void Continue() {
        gameObject.SetActive(false);
    }
}
