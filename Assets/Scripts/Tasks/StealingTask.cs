using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealingTask : Task {
    private Stealable objectToSteal;

    public Stealable ObjectToSteal {
        get {return objectToSteal;}
    }

    public StealingTask(string description, Stealable objectToSteal) {
        this.description = description;
    }
}
