using UnityEngine;

public class UIManager : MonoBehaviour {
    public GameObject settingsMenu;
    public GameObject taskListUI;

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) {
            settingsMenu.GetComponent<SettingsMenu>().ToggleMenu();
        }
        if (Input.GetKeyDown(KeyCode.Tab)) {
            taskListUI.GetComponent<TaskListUI>().ToggleTaskList();
        }
    }
}
