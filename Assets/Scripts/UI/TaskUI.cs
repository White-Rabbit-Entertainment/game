using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class TaskUI : MonoBehaviour {
    public GameObject masterTaskPrefab;
    public GameObject subTaskPrefab;
    public TaskManager taskManager;
  //  public TextMeshProUGUI allTasksTmp;

    private Color urgentTaskColour = new Color(1f, 0f, 0f, 1f);
    private Color yourTaskColor = new Color(1f, 0.14f, 0.38f, 0.85f);

    /// <summary> Adds a task to the list of tasks in the UI. </summary>
    public void AddTask(Task task, GameObject taskPrefab) {
      // Instantiate a new task list item
      GameObject item = Instantiate(taskPrefab, gameObject.transform);

      // Get the togggle component 
      TextMeshProUGUI text = item.GetComponentInChildren<TextMeshProUGUI>();
      // Set the toggle text
      text.text = task.description;
      Image image = item.GetComponentInChildren<Image>();
      if (task.timer != Timer.None && task.timer.IsStarted()) {
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
