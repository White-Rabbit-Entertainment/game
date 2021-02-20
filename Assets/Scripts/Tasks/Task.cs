using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Task {
    public bool isCompleted = false;
    public string description;

    public void Complete() {
        isCompleted = true;
    }
}
