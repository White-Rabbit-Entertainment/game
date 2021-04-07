using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackButton : MonoBehaviour {
   
    public MenuPage page;      // Where you go 
    Button button;

    void Start() {
        button = GetComponent<Button>(); 
        button.onClick.AddListener(page.Open);
    }
}
