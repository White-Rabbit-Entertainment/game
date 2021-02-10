using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerCountdown : MonoBehaviour {

    public Text timerText;
    private GameManager gameManager;

    void Start() {
        gameManager = new GameManager();
    }

    void Update() {
        timerText.text = gameManager.TimeRemaining().ToString();
    }
}


