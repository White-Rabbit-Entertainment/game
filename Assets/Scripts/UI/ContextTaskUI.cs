using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary> UI to show tasks in the GameScene </summary>
public class ContextTaskUI : TaskUI {

    private Task task;
  
    /// <summary> Adds a task to the list of tasks in the UI. </summary>
    public void ShowTask() {
      UnshowTask(); 
      AddTask(task, masterTaskPrefab);
    }

    public void SetTask(Task task) {
      if (task != this.task) {
        this.task = task;
        ShowTask();
      }
    }
    
    public void RemoveTask() {
      this.task = null;
      UnshowTask();
    }
    
    public void UnshowTask() {
      tasksList.DestroyChildren();
    }
}
