using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary> UI to show tasks in the GameScene </summary>
public class ContextTaskUI : TaskUI {

    private Task task;
  
    /// <summary> Adds a task to the list of tasks in the UI. </summary>

    public void Init(){
      if (NetworkManager.instance.GetMe() is Traitor){
        yourTask.SetActive(false);
      }
      // yourTask.SetActive(false);
    }
    public void ShowTask() {
      UnshowTask(); 
      if (NetworkManager.traitorNames.Contains(NetworkManager.myCharacter.ToString())){
        yourTask.SetActive(false);
      }
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
      taskList.DestroyChildren();
    }
}
