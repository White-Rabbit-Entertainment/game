using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskCompletionUI : MonoBehaviour {
  public TextMeshProUGUI taskCompletionText;
  public TaskManager taskManager;
  public Slider slider;
  
  public void UpdateBar() {
    taskCompletionText.text = $"Tasks Completed: {taskManager.NumberOfTasksCompleted()} / {taskManager.tasks.Count}";
    if (taskManager.tasks.Count > 0) {
      slider.value = taskManager.NumberOfTasksCompleted() / (float)taskManager.tasks.Count;
    }
  }
}
