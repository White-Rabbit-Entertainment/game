using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tasks {
    public List<Task> tasks;

    public Tasks(List<Task> tasks) {
        this.tasks = tasks;
    }
}
