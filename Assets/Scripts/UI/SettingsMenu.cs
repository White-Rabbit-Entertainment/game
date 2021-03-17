using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {

    private bool menuOpen = false;

    public Slider mouseSensitivitySlider;
    public GameObject settingsPanel;

    // Start is called before the first frame update
    void Start() {
        mouseSensitivitySlider.onValueChanged.AddListener (delegate {ValueChangeCheck ();}); 
    }
    
    public void OpenMenu() {
      bool menuOpen = true;
      settingsPanel.SetActive(true);
      Cursor.lockState = CursorLockMode.None;
    }
    
    public void CloseMenu() {
      settingsPanel.SetActive(false);
      Cursor.lockState = CursorLockMode.Locked;
      bool menuOpen = false;
    }
    
    public void ValueChangeCheck() {
      Debug.Log(mouseSensitivitySlider.value);
    }

    public void Update() {
      if (Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.P)){
        OpenMenu();
      }
    }
}
