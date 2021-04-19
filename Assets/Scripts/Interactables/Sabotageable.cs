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
    
    // private GameSceneManager gameSceneManager;
    public GameSceneManager gameSceneManager;
    
    // Start is called before the first frame update
    void Start()
    {
        // TODO Make all sabotagables glow red for traitors when not sabotaged
        isSabotaged = false;
        base.Start();
        // gameSceneManager = GameObject.Find("/GameSceneManager").GetComponent<GameSceneManager>();
    }

    void Update() {
        if (PhotonNetwork.LocalPlayer.IsMasterClient && isSabotaged && numberOfPlayersFixing > 0) {
            Debug.Log(numberOfPlayersFixing);
            amountToFix -= numberOfPlayersFixing * 5 * Time.deltaTime;
            if (amountToFix <= 0) View.RPC("Fix", RpcTarget.All); 
        }
        if (isSabotaged && Timer.SabotageTimer.IsComplete()) {
            gameSceneManager.EndGame(Team.Traitor);
        }
    }

    private void Reset() {
        team = Team.Traitor;
        taskTeam = Team.Real;
        base.Reset();
    }

    public override void PrimaryInteraction(Character character) {
        if (!isSabotaged && character.team == Team.Traitor && !Timer.SabotageTimer.IsStarted()) {
            Timer.SabotageTimer.Start(30);
            View.RPC("Sabotage", RpcTarget.All);
        } else if (isSabotaged && (Team.Real | Team.Ghost).HasFlag(character.team)) {
            task.CompleteRPC(false);
            // View.RPC("Fix", RpcTarget.All, character.GetComponent<PhotonView>().ViewID);
            Timer.SabotageTimer.End();
            fixing = true;
            View.RPC("IncrementNumberOfFixers", PhotonNetwork.MasterClient);
            Reset();
        }
    }
    
    public override void PrimaryInteractionOff(Character character) {
        if (fixing) {
            fixing = false;
            View.RPC("DecrementNumberOfFixers", PhotonNetwork.MasterClient);
        }
    }

    [PunRPC]
    public void IncrementNumberOfFixers() {
        numberOfPlayersFixing++;
    }

    [PunRPC]
    public void DecrementNumberOfFixers() {
        numberOfPlayersFixing--;
    }
    public override bool CanInteract(Character character) {
        if (!isSabotaged && character.team == Team.Traitor && !Timer.SabotageTimer.IsStarted()) return true;
        if (isSabotaged && (Team.Real | Team.Ghost).HasFlag(character.team)) return true;
        return false;
        
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
        task.description = "Fix the " + this.name + "";
        isSabotaged = true;
        sabotagedIndicator.SetActive(true);
        EnableTaskMarker();  
    }

    [PunRPC]
    public virtual void Fix() {
        // If the sabotagable is fully fixed
            isSabotaged = false;
            // Tell everyone that the task is now completed
            // TODO Delete the task for everyone
            task = null;
            Destroy(GetComponent<Task>());
            sabotagedIndicator.SetActive(false);
            DisableTaskMarker();


            // After an item is fixed its no longer interactable for anyone
            Destroy(this);
    
    }
}
