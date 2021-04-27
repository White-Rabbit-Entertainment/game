using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary> UI to show tasks in the GameScene </summary>
public class CurrentTaskUI : MonoBehaviour {

    [SerializeField] TMP_Text taskText;

    /// <summary> Adds a task to the list of tasks in the UI. </summary>
    public void Init() {
      if (NetworkManager.instance.GetMe() is Traitor){
        gameObject.SetActive(false);
      }
    }

    public void SetTask(Task task) {
      taskText.text = task.description;
    }
}
