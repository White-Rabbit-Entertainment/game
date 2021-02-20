using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tasks {
    public List<Task> tasks;

    [System.Serializable]
    public class TaskTransporter {
      public string task;
      public string type;

      public TaskTransporter(string task, string type) {
        this.task = task;
        this.type = type;
      }
    }

    public Tasks(List<Task> tasks) {
        this.tasks = tasks;
    }

    public List<string> FromJson(string jsonString) {
      TaskTransporter[] tts = JsonListHelper.FromJson<TaskTransporter>(jsonString);
      foreach(TaskTransporter tt in tts) {
        Debug.Log("Hello");
      }
      return new List<string>();
    }

    public List<string> ToJson() {
      List<string> jsonTasks = new List<string>();
      foreach (Task task in tasks) {
        string taskJsonString = JsonUtility.ToJson(task);
        TaskTransporter tt = new TaskTransporter(taskJsonString, task.GetType().ToString());
        string ttJsonString = JsonUtility.ToJson(tt);
         
        jsonTasks.Add(ttJsonString);
        Debug.Log(ttJsonString);
      }
      return jsonTasks;
    }

    public Tasks(List<string> jsonString) {
    }
}
