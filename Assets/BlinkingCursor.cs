using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
public class BlinkingCursor : MonoBehaviour {  
    public TMP_InputField playerNameInput;    
    public TextMeshProUGUI placeholder;
	private float m_TimeStamp;
	private bool cursor = false;
	private string cursorChar = "";
	private int maxStringLength = 24;
    
    // void Start(){
    //     playerNameInput.Select();
    //     playerNameInput.ActivateInputField();    
    // }
 
    // void Update() { 
    //         if (Time.time - m_TimeStamp >= 0.5)
    //     {
    //         m_TimeStamp = Time.time;
    //         if (cursor == false)
    //         {
    //             cursor = true;
    //             if (playerNameInput.text.Length < maxStringLength)
    //             {
    //                 cursorChar += "|";
    //                 placeholder.text = cursorChar;   
    //             }
    //         }
    //         else
    //         {
    //             cursor = false;
    //             if (cursorChar.Length != 0)
    //             {
    //                 cursorChar = cursorChar.Substring(0, cursorChar.Length - 1);

    //             }
    //         }
    //     }
    // }
     
}
