using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Sabotageable : Interactable {

    public bool isSabotaged;
    public int numberOfPlayersToFix = 1;
    
    public List<PlayableCharacter> playersThatFixed = new List<PlayableCharacter>();
    
    // Start is called before the first frame update
    void Start()
    {
        isSabotaged = false;
        base.Start();
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
            task.CompleteRPC();
            View.RPC("Fix", RpcTarget.All, character.GetComponent<PhotonView>().ViewID);
            Timer.SabotageTimer.End();
            Reset();
        }
    }

    [PunRPC]
    void Sabotage() {
        AddTaskWithTimerRPC(Timer.SabotageTimer);
        task.description = "Fix the " + this.name + "";
        isSabotaged = true;
    }

    [PunRPC]
    void Fix(int fixPlayerViewId) {
        PlayableCharacter fixPlayer = PhotonView.Find(fixPlayerViewId).GetComponent<PlayableCharacter>();
        playersThatFixed.Add(fixPlayer);
        if (numberOfPlayersToFix - playersThatFixed.Count <= 0) {
            isSabotaged = false;
            // Tell everyone that the task is now completed
            task.Complete();
        }
    }
}
