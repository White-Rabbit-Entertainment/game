using UnityEngine;

public class UIManager : MonoBehaviour {
    public GameObject settingsMenu;

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) {
            settingsMenu.GetComponent<SettingsMenu>().ToggleMenu();
        }
    }
}
