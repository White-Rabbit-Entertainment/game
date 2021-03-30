using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TeleType : MonoBehaviour
{
    public TextMeshProUGUI textComponent;

    // void Start(){
    //   textComponent = gameObject.GetComponent<TextMeshProUGUI>();
    // }

    public IEnumerator RevealCharacters()
       {
           textComponent = gameObject.GetComponent<TextMeshProUGUI>();

           textComponent.ForceMeshUpdate();
           TMP_TextInfo textInfo = textComponent.textInfo;

           int totalVisibleCharacters = textInfo.characterCount; // Get # of Visible Character in text object
           Debug.Log("totalVisibleCharacters: " + totalVisibleCharacters);
           int counter = 0;
           int visibleCount = 0;

           while (true)
           {
               visibleCount = counter % (totalVisibleCharacters + 1);

               textComponent.maxVisibleCharacters = visibleCount; // How many characters should TextMeshPro display?

               // Once the last character has been revealed, wait 1.0 second and start over.
               if (visibleCount >= totalVisibleCharacters)
               {
                   yield return new WaitForSeconds(10f);
               }

               counter += 1;

               yield return new WaitForSeconds(0.05f);
           }

       }
}
