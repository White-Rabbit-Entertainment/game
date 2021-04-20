using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Sabotageable : Interactable {

    public bool isSabotaged;
    public int numberOfPlayersToFix = 1;

    public bool fixing = false;

    public int numberOfPlayersFixing = 0;
    public GameObject sabotagedIndicator;

    public float amountToFix = 100f;

    public List<PlayableCharacter> playersThatFixed = new List<PlayableCharacter>();
    
    private GameSceneManager gameSceneManager;

    private SabotageManager sabotageManager;
    
    
    // Start is called before the first frame update
    void Start()
    {
        // TODO Make all sabotagables glow red for traitors when not sabotaged
        isSabotaged = false;
        base.Start();
        gameSceneManager = GameObject.Find("/GameSceneManager").GetComponent<GameSceneManager>();
        sabotageManager = GameObject.Find("/SabotageManager").GetComponent<SabotageManager>();
    }

    void Update() {
        if (PhotonNetwork.LocalPlayer.IsMasterClient && isSabotaged && numberOfPlayersFixing > 0) {
            Debug.Log(numberOfPlayersFixing);
            amountToFix -= numberOfPlayersFixing * 10 * Time.deltaTime;
            if (amountToFix <= 0) {
                View.RPC("Fix", RpcTarget.All); 
                sabotageManager.SabotageFixed();
            }
        }
        // if (isSabotaged && Timer.SabotageTimer.IsComplete()) {
        //     gameSceneManager.EndGame(Team.Traitor);
        // }
    }

    private void Reset() {
        team = Team.Traitor;
        taskTeam = Team.Real;
        // base.Reset();
    }

    public override void PrimaryInteraction(Character character) {
        if (!isSabotaged && character.team == Team.Traitor && !Timer.SabotageTimer.IsStarted()) {
            Timer.SabotageTimer.Start(30);
            View.RPC("Sabotage", RpcTarget.All);
            sabotageManager.SabotageStarted();
        } else if (isSabotaged && (Team.Real | Team.Ghost).HasFlag(character.team)) {
            if (!fixing) {
            sabotageManager.LocalPlayerFixing();    
            character.Fix(this);
            Debug.Log("ON");
            fixing = true;
            View.RPC("IncrementNumberOfFixers", PhotonNetwork.MasterClient);
            }
        }
        }

    
    public override void PrimaryInteractionOff(Character character) {
        Debug.Log("PIF");
        if (fixing) {
        character.StopFix(this);
        fixing = false;
        View.RPC("DecrementNumberOfFixers", PhotonNetwork.MasterClient);
        sabotageManager.SetNumOfPlayers(numberOfPlayersFixing);
        Debug.Log(numberOfPlayersFixing);   
        } 
        // if (fixing) {
        //     Debug.Log("OFF");
        //     fixing = false;
        //     View.RPC("DecrementNumberOfFixers", PhotonNetwork.MasterClient);
        //     Debug.Log(numberOfPlayersFixing);
        // }
    }

    [PunRPC]
    public void IncrementNumberOfFixers() {
        this.numberOfPlayersFixing++;
    }

    [PunRPC]
    public void DecrementNumberOfFixers() {
        this.numberOfPlayersFixing--;
    }
    public override bool CanInteract(Character character) {
        if (!isSabotaged && character.team == Team.Traitor && !Timer.SabotageTimer.IsStarted()) return true;
        if (isSabotaged && (Team.Real | Team.Ghost).HasFlag(character.team)) return true;
        return true;
        
    }

    public override void SetTaskGlow() {
        Team team = NetworkManager.instance.GetLocalPlayerProperty<Team>("Team");
        if (inRange && team == Team.Traitor && !isSabotaged) {
            SetGlow(undoTaskColour);
        } else {
            base.SetTaskGlow();
        }
    }

    [PunRPC]
    public virtual void Sabotage() {
        AddTaskWithTimerRPC(Timer.SabotageTimer);
        task.isUndoable = false;
        task.description = "Fix the " + this.name + "";
        isSabotaged = true;
        sabotagedIndicator.SetActive(true);
        EnableTaskMarker();  
    }

    [PunRPC]
    public virtual void Fix() {
        // If the sabotagable is fully fixed
            isSabotaged = false;
            task.CompleteRPC(false);
            // Tell everyone that the task is now completed
            // TODO Delete the task for everyone
            DisableTaskMarker();
            sabotagedIndicator.SetActive(false);
            Timer.SabotageTimer.End();
            Destroy(GetComponent<Task>());
            task = null;
            // After an item is fixed its no longer interactable for anyone
            Destroy(this);
    
    }
}
