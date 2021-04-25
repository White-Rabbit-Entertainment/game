using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {

    private bool menuOpen = false;

    [SerializeField] private Slider mouseSensitivitySlider;
    [SerializeField] private GameObject settingsPanel;

    [SerializeField] private GameObject playerVolumesList;
    [SerializeField] private GameObject playerVolumeItem;

    [SerializeField] private WebRTC webRTC;

    // Start is called before the first frame update
    void Start() {
      mouseSensitivitySlider.onValueChanged.AddListener (delegate {OnMouseSensitivitySliderChange ();}); 
    }

    public void ToggleMenu() {
      if (menuOpen) {
        CloseMenu();
      } else {
        OpenMenu();
      }
    }

    public void InitPlayerVolumes() {
      foreach (PlayableCharacter player in FindObjectsOfType<PlayableCharacter>()) {
        GameObject item = Instantiate(playerVolumeItem, playerVolumesList.transform);
        item.GetComponentInChildren<Image>().color = player.Colour; 

        Slider slider = item.GetComponentInChildren<Slider>();
        slider.onValueChanged.AddListener(delegate {
          webRTC.SetVolume(player.Owner.ActorNumber, slider.value);
        });
        slider.value = webRTC.GetVolume(player.Owner.ActorNumber);
      }
    }
    
    public void OpenMenu() {
      menuOpen = true;
      mouseSensitivitySlider.value = NetworkManager.instance.GetMe().GetComponentInChildren<CameraMouseLook>().mouseSensitivity;
      NetworkManager.instance.GetMe().Freeze();
      settingsPanel.SetActive(true);
      Cursor.lockState = CursorLockMode.None;
      Cursor.visible = true;
      InitPlayerVolumes();
    }
    
    public void CloseMenu() {
      settingsPanel.SetActive(false);
      NetworkManager.instance.GetMe().Unfreeze();
      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = false;
      menuOpen = false;
    }
    
    public void OnMouseSensitivitySliderChange() {
      NetworkManager.instance.GetMe().GetComponentInChildren<CameraMouseLook>().mouseSensitivity = mouseSensitivitySlider.value;
    }

    public void Update() {
      if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)){
        ToggleMenu();
      }
    }
}
