using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public static class Utils {
    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list) {  
        int n = list.Count;  
        while (n > 1) {  
            n--;  
            int k = rng.Next(n + 1);  
            T value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }  
    }
    
    // Scroll a scrollrect to the bottom
    public static void ScrollToBottom(this ScrollRect scrollRect) {
        scrollRect.verticalNormalizedPosition = 1f;
    }

    // Clear the text of an InputFiled
    public static void Clear(this InputField inputfield)
    {
        inputfield.Select();
        inputfield.text = "";
    }
}

