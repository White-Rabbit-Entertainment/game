using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StealingTask : Task {
    public Stealable objectToSteal;

    // public Stealable ObjectToSteal {
    //     get {return objectToSteal;}
    // }

    public StealingTask(string description) {
        this.description = description;
    }
}
