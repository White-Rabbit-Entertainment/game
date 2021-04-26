using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BlinkingImage : MonoBehaviour
{
    // Start is called before the first frame update
    public Image image;
    // public GameObject SabotageUI;

    void OnEnable()
    {
        // image = GetComponent<Image>();
        // image = SabotageUI.GetComponentInChildren<Image>();
        Debug.Log("on enabled");
        StartBlinking();
    }
    IEnumerator Blink()
    {
        while (true)
        {
            var tempColor = image.color;
            switch (tempColor.a.ToString())
            {
                case "0":
                    // image.color = new Color(tempColor.r, tempColor.g, tempColor.b, 1f);
                    ChangeAlpha(image,1f);
                    yield return new WaitForSeconds(1f);
                    break;
                case "1":
                    // image.color = new Color(tempColor.r, tempColor.g, tempColor.b, 0f);
                    ChangeAlpha(image,0f);
                    yield return new WaitForSeconds(0.7f);
                    break;
            }
        }
    }

    void StartBlinking()
    {
        Debug.Log("started blinking");
        StopCoroutine("Blink");
        StartCoroutine("Blink");
    }

    void StopBlinking()
    {
        StopCoroutine("Blink");
    }

     public Image ChangeAlpha(Image g, float newAlpha)
     {
        Color col = g.color;
        col.a = newAlpha;
        g.color = col;
        return g;
     }

}
