using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : Pickupable {
    public override void PrimaryInteraction(Character character) {
        base.PrimaryInteraction(character);
        // When picked up complete the task
        if (task != null) task.Complete();
    }
    
    public override void PrimaryInteractionOff(Character character) {
        base.PrimaryInteractionOff(character);
        if (task != null) task.Uncomplete(false);
    }

    public override bool CanInteract(Character character) {
        return true;
    }
}
