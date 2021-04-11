using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FadeText : MonoBehaviour
{
    public TextMeshProUGUI tmp;
    private int x = 0;
    private float t;
    public IEnumerator FadeTextToFullAlpha(float t, TextMeshProUGUI i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
        yield return new WaitForSeconds(5f);
    }
 
    public IEnumerator FadeTextToZeroAlpha(float t,TextMeshProUGUI i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            // i.textInfo.meshInfo.colors32 = new Color32(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
        // yield return new WaitForSeconds(5f);
    }

    void Start(){
        float t = 0f;
        // StartCoroutine(FadeTextToZeroAlpha(2f, tmp));
    }
  
    void Update(){
        t++;
        // if (t == 1f){
        //     StartCoroutine(FadeTextToZeroAlpha(2f, tmp));
            
        // }
        
        if (t == 100f){
            // StartCoroutine(FadeTextToFullAlpha(100f, tmp));
            StartCoroutine(FadeTextToFullAlpha(2f, tmp));
            t=0f;
        }
      
        
        // FadeTextToFullAlpha(5f, tmp);
        // FadeTextToZeroAlpha(5f, tmp);
        // if (Input.GetKeyDown(KeyCode.Q))
        // {
        //     StartCoroutine(FadeTextToFullAlpha(1f, i));
        // }
        // if (Input.GetKeyDown(KeyCode.E))
        // {
        //     StartCoroutine(FadeTextToZeroAlpha(1f, i));
        // }
    }
}