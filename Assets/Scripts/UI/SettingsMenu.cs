using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour {

    private bool menuOpen = false;

    [SerializeField] private Slider mouseSensitivitySlider;

    [SerializeField] private GameObject playerVolumesList;
    [SerializeField] private GameObject playerVolumeItem;

    [SerializeField] private WebRTC webRTC;
    
    [SerializeField] List<GameObject> thingsToDisableForLowGraphics;
    [SerializeField] MonoBehaviour lowPolyWater;
    [SerializeField] Button graphicsButton;

    [SerializeField] GameObject highMaterialShip;
    [SerializeField] GameObject lowMaterialShip;

    
    private bool highGraphicsQuality = true;

    // Start is called before the first frame update
    void Start() {
      mouseSensitivitySlider.onValueChanged.AddListener (delegate {OnMouseSensitivitySliderChange ();}); 
      graphicsButton.onClick.AddListener(ToggleGraphics);
    }

    public void ToggleMenu() {
      if (menuOpen) {
        CloseMenu();
      } else {
        OpenMenu();
      }
    }

    public void InitPlayerVolumes() {
      playerVolumesList.DestroyChildren();

      foreach (PlayableCharacter player in FindObjectsOfType<PlayableCharacter>()) {
        if (!player.IsMe()) {
          GameObject item = Instantiate(playerVolumeItem, playerVolumesList.transform);
          foreach (Image image in item.GetComponentsInChildren<Image>()) {
            image.color = player.playerInfo.color; 
          }

          Slider slider = item.GetComponentInChildren<Slider>();
          slider.onValueChanged.AddListener(delegate {
            webRTC.SetVolume(player.Owner.ActorNumber, slider.value);
            Debug.Log("Slider value being set");
            Debug.Log(slider.value);
          });
          slider.value = webRTC.GetVolume(player.Owner.ActorNumber);
          Debug.Log(webRTC.GetVolume(player.Owner.ActorNumber));
        }
      }
    }
    
    public void OpenMenu() {
      menuOpen = true;
      mouseSensitivitySlider.value = NetworkManager.instance.GetMe().GetComponentInChildren<CameraMouseLook>().mouseSensitivity;
      NetworkManager.instance.GetMe().Freeze();
      gameObject.SetActive(true);
      Cursor.lockState = CursorLockMode.None;
      Cursor.visible = true;
      InitPlayerVolumes();
    }
    
    public void CloseMenu() {
      gameObject.SetActive(false);
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

    public void ToggleGraphics() {
      if(highGraphicsQuality) LowGraphics();
      else HighGraphics();
    }

    public void LowGraphics() {
      graphicsButton.GetComponentInChildren<TMP_Text>().text = "Low";
      highGraphicsQuality = false;
      lowPolyWater.enabled = false;
      foreach(GameObject go in thingsToDisableForLowGraphics) {
        go.SetActive(false);
      }
      lowMaterialShip.SetActive(true);
      highMaterialShip.SetActive(false);
    }
    
    public void HighGraphics() {
      graphicsButton.GetComponentInChildren<TMP_Text>().text = "High";
      highGraphicsQuality = true;
      lowPolyWater.enabled = true;
      foreach(GameObject go in thingsToDisableForLowGraphics) {
        go.SetActive(true);
      }
      highMaterialShip.SetActive(true);
      lowMaterialShip.SetActive(false);
    }
}
