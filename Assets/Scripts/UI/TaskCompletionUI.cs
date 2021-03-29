using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskCompletionUI : MonoBehaviour {
  public GameObject taskCompletionBar;
  public TaskManager taskManager;

  public void Update() {
    taskCompletionBar.GetComponentInChildren<Text>().text = $"{taskManager.NumberOfTasksCompleted()} / {taskManager.tasks.Count}";
  }
}
