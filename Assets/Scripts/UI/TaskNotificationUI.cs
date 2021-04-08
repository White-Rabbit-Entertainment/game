using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskNotificationUI : MonoBehaviour
{

    public TextMeshProUGUI notificationPrefab;
    private string taskCompleteMessage = "Task Complete";
    private string taskUndoneMessage = "Task Undone";

    public void SetNotification(bool isComplete) {
        if (isComplete) {
            notificationPrefab.text = taskCompleteMessage;
        } else if (!isComplete) {
            notificationPrefab.text = taskUndoneMessage;
        }

        StartCoroutine(RemoveNotification(1f));
    }


    IEnumerator RemoveNotification(float timeInSeconds) {
        yield return new WaitForSeconds(timeInSeconds);
        notificationPrefab.text = "";
    }
    
}
