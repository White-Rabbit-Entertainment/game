using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CountdownVote : MonoBehaviour
{
    [SerializeField] private Image countdownCircleTimer;
    [SerializeField] private TextMeshProUGUI countdownText;
    private float startTime;
     private float currentTime;
     private bool updateTime;
     private void Start()
     { 
        currentTime = (float)Timer.VoteTimer.TimeRemaining();
        startTime = currentTime;
        countdownCircleTimer.fillAmount = 1.0f;
         // Easy way to represent only the seconds and skip the
         // float     
        countdownText.text = (int)currentTime+ "s";
         // update the countdown on the update
        updateTime = true;
     }
      private void Update()
      {
          if (updateTime)
          {
            // currentTime -= Time.deltaTime;
            currentTime = (float)Timer.VoteTimer.TimeRemaining();
            if (Timer.VoteTimer.IsComplete())
            {
               // Stop the countdown timer              
               updateTime = false;
               currentTime = 0.0f;
            }
            countdownText.text = (int)currentTime + "s";
            float normalizedValue = Mathf.Clamp(currentTime /startTime, 0.0f, 1.0f);
            countdownCircleTimer.fillAmount = normalizedValue;
           }
    }
}
