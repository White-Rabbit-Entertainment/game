using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerCountdown : MonoBehaviour {

    public TextMeshProUGUI timerText;
    public bool stopped = false;

    void Update() {
        if (!stopped) {
            timerText.text = Timer.RoundTimer.FormatTime();
        }
    }

    public void Stop() {
        stopped = true;
    }
}


