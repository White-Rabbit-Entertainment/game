using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class AmountFixed : MonoBehaviour
{
    [SerializeField] private Image countdownCircle;
    [SerializeField] private TextMeshProUGUI countdownText;
    public SabotageManager sabotageManager;

    // private float fillAmount = 1.0f;
    private void OnEnable() { 
      sabotageManager = GameObject.Find("/SabotageManager").GetComponent<SabotageManager>();
      Sabotageable sabotageable = sabotageManager.sabotageable;
    }

    private void Update() {
       Sabotageable sabotageable = sabotageManager.sabotageable;
       if (sabotageable != null) {
         Debug.Log("Setting values");
         SetValues(sabotageable);
       }
    }

    public void SetValues(Sabotageable sabotageable) {
      float fractionCompleted = sabotageable.startingAmountToFix - sabotageable.amountToFix;
      countdownCircle.fillAmount = fractionCompleted / sabotageable.startingAmountToFix;
      countdownText.text = (int)fractionCompleted + "%";
    }
}
