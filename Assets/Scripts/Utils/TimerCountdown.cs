using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerCountdown : MonoBehaviour {

    public Text timerText;

    void Update() {
        int secondsLeft = (int)GameManager.instance.TimeRemaining();
        int minutes = secondsLeft / 60;
        int seconds = secondsLeft % 60;
        string secondsStr = seconds.ToString();
        string minutesStr = minutes.ToString();
        if (minutes < 10) minutesStr = "0" + minutesStr;
        if (seconds < 10) secondsStr = "0" + secondsStr;
        timerText.text = minutesStr + ":" + secondsStr;
    }
}


