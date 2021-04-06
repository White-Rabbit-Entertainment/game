using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackButton : MonoBehaviour {
   
    public GameObject currentGO;   // Where you are
    public GameObject backGO;      // Where you go 

    Button button;

    void Start() {
        button = GetComponent<Button>(); 
        button.onClick.AddListener(GoBack);
    }

    void GoBack() {
        if (currentGO != null) {
            currentGO.SetActive(false);
        }
        if (backGO != null) {
            backGO.SetActive(true);
        }
    }
}
