using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Task {
    protected bool isCompleted = false;
    protected string description;

    public string Description {
        get {return description;}
    }

    public bool IsCompleted {
        get {return isCompleted;}
    }

    public void Complete() {
        isCompleted = true;
    }
}
