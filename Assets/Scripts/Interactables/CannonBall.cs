using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CannonBall : Pickupable {
    bool inCannon = false; // Whether the cannonball is current in a cannon
    GameSceneManager gameSceneManager;

    // This is used for cannons with tasks. Cannons cannot be a master task,
    // therefore they are always going to be the child of a Cannon.
    public Cannon Cannon {
        get { 
            if (task != null && task.parent != null) {
                return task.parent.GetComponent<Cannon>();
            }
            return null;
        }
    }

    public override void Start() {
        base.Start();
        gameSceneManager = GameObject.Find("/GameSceneManager").GetComponent<GameSceneManager>();
    }
    
    public override void Reset() {
        canBeMasterTask = false;
        softRequirementProbability = 1.0f;
    }

    public override void PrimaryInteraction(Character character) {
        base.PrimaryInteraction(character);

        // When picked up 
        if (task != null) {
            // Complete the task 
            task.Complete();
            // Enable the pickup destination for the cannon 
            Cannon.GetComponent<PickupDestination>().EnableDestinationZone();
        }
    }
    
    public override void PrimaryInteractionOff(Character character) {
        base.PrimaryInteractionOff(character);
        if (task != null && !inCannon && character is PlayableCharacter) {
            task.Uncomplete(false);
            // Reassign the cannon ball to the character (since they currently
            // have the  cannon as their task)
            task.AssignSubTaskToCharacter((PlayableCharacter)character);
        }
    }

    // When a cannonball collides with its cannon it should be completed
    void OnCollisionEnter(Collision collision) {
        if(Cannon != null && Cannon.gameObject == collision.gameObject) {
            task.parent.Complete();
            Cannon.DisableDestinationZone();
        }
    }
  
    public override void OnParentTaskComplete(Character character = null) {
        View.RPC("SetInCannonConditions", RpcTarget.All);
    }

    public override void OnParentTaskUncomplete() {
        View.RPC("SetOutCannonConditions", RpcTarget.All, gameSceneManager.RandomNavmeshLocation());
        task.Uncomplete();
    }

    // When a cannonball is in a cannon it should be consumed by the cannon
    // (inactive).
    [PunRPC]
    public void SetInCannonConditions() {
        inCannon = true;
        task.isCompleted = true;
        gameObject.SetActive(false);
    }
    
    [PunRPC]
    public void SetOutCannonConditions(Vector3 newPosition) {
        gameObject.SetActive(true);
        inCannon = false;
        transform.position = newPosition;

        // Remove all velocity (prevents strange bouncy stuff)
        GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
        GetComponent<Rigidbody>().angularVelocity = new Vector3(0,0,0);
    }

    public override void AddTask(Task parentTask = null) {
        base.AddTask(parentTask);
        task.isUndoable = false;
    }
}
