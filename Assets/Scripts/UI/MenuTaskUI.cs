using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary> UI to show tasks in the GameScene </summary>
public class MenuTaskUI : TaskUI {
  void Update() {
    tasksList.DestroyChildren();
    // Add in all the current tasks
    foreach (Task task in taskManager.GetTasks()) {
      if (task.IsMasterTask()) {
        AddTask(task, masterTaskPrefab);
        foreach(Task subTask in task.requirements) {
          AddTask(subTask, subTaskPrefab);
        }
      }
    }
  }
}
