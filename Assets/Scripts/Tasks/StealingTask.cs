using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealingTask : Task {
    public StealingTask(string description) {
        this.description = description;
    }
    public override void Complete() {
        Debug.Log("Completed");
    }
}
