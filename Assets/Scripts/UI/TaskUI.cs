using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class TaskUI : MonoBehaviour {
    public GameObject masterTaskPrefab;
    public GameObject subTaskPrefab;
    public GameObject taskList;
    public TaskManager taskManager;

    private Color urgentTaskColour = new Color(1f, 0f, 0f, 1f);

    /// <summary> Adds a task to the list of tasks in the UI. </summary>
    public void AddTask(Task task, GameObject taskPrefab) {
      // Instantiate a new task list item
      GameObject item = Instantiate(taskPrefab, taskList.transform);

      // Get the togggle component 
      TMP_Text text = item.GetComponentInChildren<TMP_Text>();
      // Set the toggle text
      text.text = task.description;
     
      if (task.timer != Timer.None && task.timer.IsStarted()) {
        text.text += task.timer.FormatTime();
        Image image = item.GetComponentInChildren<Image>();
        image.color = urgentTaskColour;
      }
      
      // Set the toggle on/off depending of if task is completed 
      if (task.isCompleted) {
        text.fontStyle = FontStyles.Strikethrough;
      }
    }
}
