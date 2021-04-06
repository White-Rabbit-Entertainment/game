using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackButton : MonoBehaviour {
   
    public GameObject currentGO;   // Where you are
    public GameObject backGO;      // Where you go 

    public bool changeScene = false;
    public string newScene;

    Button button;

    void Start() {
        button = GetComponent<Button>(); 
        button.onClick.AddListener(GoBack);
    }

    void GoBack() {
        if (changeScene) {
            NetworkManager.instance.ChangeScene(newScene);
        } else {
            currentGO.SetActive(false);
            backGO.SetActive(true);
        }
    }
}
