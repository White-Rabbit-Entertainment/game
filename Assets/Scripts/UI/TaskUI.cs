using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary> UI to show tasks in the GameScene </summary>
public class TaskUI : MonoBehaviour {
    
    public GameObject togglePrefab;
    public GameObject tasksList;

    void Update() {
      EmptyList();
      
      // Add in all the current tasks
      foreach (Task task in GameManager.instance.GetTasks()) {
        AddTask(task);
      }
    }

    /// <summary> Removes all tasks from the list. </summary>
    void EmptyList() {
      foreach (Transform child in tasksList.transform) {
        Destroy(child.gameObject);
      }
    }
  
    /// <summary> Adds a task to the list of tasks in the UI. </summary>
    void AddTask(Task task) {
      // Instantiate a new task list item
      GameObject item = Instantiate(togglePrefab, tasksList.transform);

      // Get the togggle component 
      Toggle toggle = item.GetComponentInChildren<Toggle>();
      // Set the toggle text
      toggle.GetComponentInChildren<Text>().text = task.description;
      // Set the toggle on/off depending of if task is completed 
      toggle.isOn = task.isCompleted;
    }
}
