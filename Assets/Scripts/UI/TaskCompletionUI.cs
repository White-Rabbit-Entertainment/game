using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskCompletionUI : MonoBehaviour {
  public GameObject taskCompletionBar;
  public TaskManager taskManager;
  public Slider slider;

  public void Update() {
    taskCompletionBar.GetComponentInChildren<Text>().text = $"{taskManager.NumberOfTasksCompleted()} / {taskManager.tasks.Count}";
    slider.value = taskManager.NumberOfTasksCompleted() / taskManager.tasks.Count;
  }
}
