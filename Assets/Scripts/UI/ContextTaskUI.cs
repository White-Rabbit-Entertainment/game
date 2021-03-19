using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary> UI to show tasks in the GameScene </summary>
public class ContextTaskUI : MonoBehaviour {
    
    public GameObject taskItemPrefab;
    public GameObject tasksList;

    /// <summary> Removes all tasks from the list. </summary>
    void EmptyList() {
      foreach (Transform child in tasksList.transform) {
        Destroy(child.gameObject);
      }
    }
  
    /// <summary> Adds a task to the list of tasks in the UI. </summary>
    void AddTask(Task task) {
      // Instantiate a new task list item
      GameObject item = Instantiate(taskItemPrefab, tasksList.transform);

      // Get the togggle component 
      TMP_Text text = item.GetComponentInChildren<TMP_Text>();
      // Set the toggle text
      text.text = task.description;
      // Set the toggle on/off depending of if task is completed 
      if (task.isCompleted) {
        text.fontStyle = FontStyles.Strikethrough;
      }
    }
}
