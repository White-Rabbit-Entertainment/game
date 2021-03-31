using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TaskNotificationUI : MonoBehaviour
{

    public TextMeshProUGUI notificationPrefab;
    public string taskCompleteMessage;
    public string taskUndoneMessage;

    public void SetNotification(bool isComplete) {
        if (isComplete) {
            notificationPrefab.SetText(taskCompleteMessage);
        } else {
            notificationPrefab.SetText(taskUndoneMessage);
        }

        StartCoroutine(RemoveNotification(2f));
    }


    IEnumerator RemoveNotification(float timeInSeconds) {
        yield return new WaitForSeconds(timeInSeconds);
        notificationPrefab.SetText("");
    }
    
}
