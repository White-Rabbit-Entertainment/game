using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerCountdown : MonoBehaviour {

    public Text timerText;

    void Update() {
        timerText.text = Timer.RoundTimer.FormatTime();
    }
}


