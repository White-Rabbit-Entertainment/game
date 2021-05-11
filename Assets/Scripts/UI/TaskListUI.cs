using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary> UI to show tasks in the GameScene </summary>
public class TaskListUI : MonoBehaviour {

  private bool taskListOpen = false;
  public GameObject masterTaskPrefab;
  public GameObject subTaskPrefab;
  public TaskManager taskManager;

  public GameObject allTasksUI;
    
  private Color urgentTaskColour = new Color(1f, 0f, 0f, 1f);
  private Color yourTaskColor = new Color(1f, 0.14f, 0.38f, 0.85f);

  void Update() {
    gameObject.DestroyChildren();
    // Add in all the current tasks
    foreach (Task task in taskManager.GetTasks()) {
      if (task.IsMasterTask()) {
        AddTask(task, masterTaskPrefab);
        foreach(Task subTask in task.requirements) {
          AddTask(subTask, subTaskPrefab);
        }
      }
    }

    if (Input.GetKeyDown(KeyCode.Tab)) {
      ToggleTaskList();
    }
  }

  public void ToggleTaskList() {
    if (taskListOpen) {
      allTasksUI.SetActive(false);
      taskListOpen = false;
    } else {
      allTasksUI.SetActive(true);
      taskListOpen = true;
    }
  }
    
  /// <summary> Adds a task to the list of tasks in the UI. </summary>
  public void AddTask(Task task, GameObject taskPrefab) {
    // Instantiate a new task list item
    GameObject item = Instantiate(taskPrefab, gameObject.transform);

    // Get the togggle component 
    TextMeshProUGUI text = item.GetComponentInChildren<TextMeshProUGUI>();
    // Set the toggle text
    text.text = task.description;
    Image image = item.GetComponentInChildren<Image>();
    if (task.timer != null && task.timer.IsStarted()) {
      text.text += task.timer.FormatTime();
      image.color = urgentTaskColour;
    }
    
    // Set the toggle on/off depending of if task is completed 
    if (task.isCompleted) {
      item.transform.Find("tick").gameObject.SetActive(true);
    } else {
      item.transform.Find("tick").gameObject.SetActive(false);
    }

    if (NetworkManager.instance.GetMe().assignedSubTask == task){
      image.color = yourTaskColor;
    }

  }
}
