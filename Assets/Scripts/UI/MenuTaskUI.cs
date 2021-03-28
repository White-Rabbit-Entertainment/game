﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary> UI to show tasks in the GameScene </summary>
public class MenuTaskUI : TaskUI {
  private bool taskListOpen = false;

  void Update() {
    taskList.DestroyChildren();
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

  void ToggleTaskList() {
    if (taskListOpen) {
      taskList.SetActive(false);
      taskListOpen = false;
    } else {
      taskList.SetActive(true);
      taskListOpen = true;
    }
  }
}
