using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerCountdown : MonoBehaviour
{
    //text representing in game timer
    public GameObject textDisplay;
    //Initialise timer at 10 minutes
    public int secondsLeft = 600;
    public bool takingAway = false;

    // Start is called before the first frame update
    void Start()
    {   
        //Timer logic to go from seconds to minutes and seconds
        int minutes = secondsLeft / 60;
        int seconds = secondsLeft % 60;
        string secondsStr = seconds.ToString();
        string minutesStr = minutes.ToString();
        if (minutes < 10) minutesStr = "0" + minutesStr;
        if (seconds < 10) secondsStr = "0" + minutesStr;
        textDisplay.GetComponent<Text>().text = minutesStr + ":" + secondsStr;
    }

    // Update is called once per frame
    void Update()
    {   
        //Update timer
        if (!takingAway && secondsLeft > 0) StartCoroutine(TimerTake()); 
        //Once timer hits zero state game over (ready to be extended for end game logic, can launch another function to evaluate gamestate)
        if (secondsLeft <= 0) textDisplay.GetComponent<Text>().text = "GAME OVER"; 
    }

    IEnumerator TimerTake() {
        //Make timer wait one second before subtracting 1 from seconds
        takingAway = true;
        yield return new WaitForSeconds(1);
        secondsLeft -= 1;
        //Timer logic to go from seconds to minutes and seconds
        int minutes = secondsLeft / 60;
        int seconds = secondsLeft % 60;
        string secondsStr = seconds.ToString();
        string minutesStr = minutes.ToString();
        if (minutes < 10) minutesStr = "0" + minutesStr;
        if (seconds < 10) secondsStr = "0" + minutesStr;
        textDisplay.GetComponent<Text>().text = minutesStr + ":" + secondsStr;
        takingAway = false;
    }
}
