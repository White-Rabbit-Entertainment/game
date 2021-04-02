using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerCountdown : MonoBehaviour {

    public TextMeshProUGUI timerText;

    void Update() {
        timerText.text = Timer.RoundTimer.FormatTime();
    }
}


