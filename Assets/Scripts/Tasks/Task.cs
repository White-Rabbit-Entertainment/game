using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Task {
    protected string description;
    public string Description {
        get {return description;}
    }

    public abstract void Complete();
}
