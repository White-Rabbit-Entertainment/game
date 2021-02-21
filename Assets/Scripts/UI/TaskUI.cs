using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskUI : MonoBehaviour {
    
    public GameObject togglePrefab;
    public GameObject tasksList;

    private Task[] tasks;

    void Start() {
      tasks = new Task[10];
      Debug.Log("Starting");
      Task task = new Task();
      task.description = "Testeroo";
      tasks[0] = task;
    }

    void Update() {
      EmptyList();
      foreach (Task task in GameManager.instance.GetTasks()) {
        AddTask(task);
      }
    }

    void EmptyList() {
      foreach (Transform child in tasksList.transform) {
        Destroy(child.gameObject);
      }
    }

    void AddTask(Task task) {
      GameObject item = Instantiate(togglePrefab, tasksList.transform);
      Toggle toggle = item.GetComponentInChildren<Toggle>();
      toggle.GetComponentInChildren<Text>().text = task.description;
      toggle.isOn = task.isCompleted;
    }
}
