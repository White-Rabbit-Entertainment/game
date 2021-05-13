using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CannonBall : Pickupable {
    // Cannons cannot be a master task, therefore they are always going to be
    // the child of a Cannon.
    
    bool inCannon = false;
    GameSceneManager gameSceneManager;

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
        // When picked up complete the task
        if (task != null) {
            task.Complete();
            Cannon.GetComponent<PickupDestination>().EnableDestinationZone();
        }
    }
    
    public override void PrimaryInteractionOff(Character character) {
        base.PrimaryInteractionOff(character);
        if (task != null && !inCannon && character is PlayableCharacter) {
            task.Uncomplete(false);
            task.AssignSubTaskToCharacter((PlayableCharacter)character);
        }
    }

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
    }

    public override void AddTask(Task parentTask = null) {
        base.AddTask(parentTask);
        task.isUndoable = false;
    }
}
