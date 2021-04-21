using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class AmountFixed : MonoBehaviour
{
    [SerializeField] private Image countdownCircle;
    [SerializeField] private TextMeshProUGUI countdownText;
    private float startAmountToFix;
    private float currentAmountToFix;
    private bool updateAmountToFix;
    public SabotageManager SabotageManager;

    // private float fillAmount = 1.0f;
     private void OnEnable()
     { 
        currentAmountToFix = SabotageManager.GetAmountToFix();
        startAmountToFix = 100f;
        countdownCircle.fillAmount = ((startAmountToFix-currentAmountToFix)/1.0f);
         // Easy way to represent only the seconds and skip the
         // float     
        countdownText.text = (int)((startAmountToFix-currentAmountToFix)/1.0f)+ "%";
         // update the countdown on the update
        updateAmountToFix = true;
     }
      private void Update()
      {
          if (SabotageManager.GetIsFixing())
          {
            // currentAmountToFix -= Time.deltaTime;
            currentAmountToFix = SabotageManager.GetAmountToFix();
            if (SabotageManager.amountToFix <= 0f)
            {
               // Stop the countdown               
            //    updateAmountToFix = false;
               currentAmountToFix = 0.0f;
            }
            countdownText.text = (int)((startAmountToFix-currentAmountToFix)/1.0f)+ "%";
            float normalizedValue = Mathf.Clamp(currentAmountToFix /startAmountToFix, 0.0f, 1.0f);
            // fillAmount = normalizedValue;
            countdownCircle.fillAmount = normalizedValue;
           }
    }
}