using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StealingTask : Task {
    public Transform objectToSteal;

    // public Stealable ObjectToSteal {
    //     get {return objectToSteal;}
    // }

    public StealingTask(string description, Transform item) {
        this.description = description;
        objectToSteal = item;
    }
}
