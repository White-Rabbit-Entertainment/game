using UnityEngine;
using TMPro;

public abstract class TaskUI : MonoBehaviour {
    public GameObject masterTaskPrefab;
    public GameObject subTaskPrefab;
    public GameObject tasksList;
    public TaskManager taskManager;

    /// <summary> Adds a task to the list of tasks in the UI. </summary>
    public void AddTask(Task task, GameObject taskPrefab) {
      // Instantiate a new task list item
      GameObject item = Instantiate(taskPrefab, tasksList.transform);

      // Get the togggle component 
      TMP_Text text = item.GetComponentInChildren<TMP_Text>();
      // Set the toggle text
      text.text = task.description;

      if (task.timer != null && task.timer.IsStarted()) {
        text.text += Timer.SabotageTimer.TimeRemaining();
      }
      // Set the toggle on/off depending of if task is completed 
      if (task.isCompleted) {
        text.fontStyle = FontStyles.Strikethrough;
      }
    }
}
