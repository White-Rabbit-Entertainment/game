using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public static class Utils {
    private static System.Random rng = new System.Random(System.Guid.NewGuid().GetHashCode());

    public static void SetLayerRecursively(this GameObject go, int layerNumber) {
      foreach (Transform trans in go.GetComponentsInChildren<Transform>(true)) {
        trans.gameObject.layer = layerNumber;
      }
    }


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
    
    // Clear the text of an InputFiled
    public static void Clear(this InputField inputfield)
    {
        inputfield.Select();
        inputfield.text = "";
    }
    
    public static void DestroyChildren(this GameObject gameObject) {
      foreach (Transform child in gameObject.transform) {
        UnityEngine.Object.Destroy(child.gameObject);
      }
    }

}
