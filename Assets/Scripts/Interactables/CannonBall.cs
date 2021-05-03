using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CannonBall : Pickupable {
    // Cannons cannot be a master task, therefore they are always going to be
    // the child of a Cannon.
    
    bool inCannon = false;
    
    public override void Reset() {
        canBeMasterTask = false;
        softRequirementProbability = 1.0f;
    }

    public override void PrimaryInteraction(Character character) {
        base.PrimaryInteraction(character);
        // When picked up complete the task
        if (task != null) task.Complete();
    }
    
    public override void PrimaryInteractionOff(Character character) {
        base.PrimaryInteractionOff(character);
        if (task != null && !inCannon && character is PlayableCharacter) {
            task.Uncomplete(false);
            task.AssignSubTaskToCharacter((PlayableCharacter)character);
        }
    }

    public override bool CanInteract(Character character) {
        return true;
    }

    void OnCollisionEnter(Collision collision) {
        Debug.Log("collision");
        if(task != null && collision.gameObject == task.parent.gameObject) {
            Debug.Log("collision of right thing");
            task.parent.Complete();
        }
    }
  
    public override void OnParentTaskComplete(Character character = null) {
        View.RPC("SetInCannonConditions", RpcTarget.All);
    }

    public override void OnParentTaskUncomplete() {
        View.RPC("SetOutCannonConditions", RpcTarget.All, new Vector3(0f, 1f, 0f));
        task.Uncomplete();
    }

    [PunRPC]
    public void SetInCannonConditions() {
        inCannon = true;
        gameObject.SetActive(false);
    }
    
    [PunRPC]
    public void SetOutCannonConditions(Vector3 newPosition) {
        gameObject.SetActive(true);
        transform.position = newPosition;
    }
}
