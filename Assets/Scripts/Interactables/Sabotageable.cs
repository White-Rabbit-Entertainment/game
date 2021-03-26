using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Sabotageable : Interactable {

    public bool isSabotaged;

    public int initialNumberOfPlayersToFix;

    public int numberOfPlayersToFix;

    // private void Dictionary<string, string> openWith = new Dictionary<string, string>();

    // Start is called before the first frame update
    void Start()
    {
        isSabotaged = false;
        base.Start();
    }

    // public override bool CanInteract(Character character) {
    //     if (character is Traitor && Timer.Sabotage.IsStarted()) return false;
    //     return base.CanInteract(character);
    // }

    private void Reset() {
        team = Team.Traitor;
        taskTeam = Team.Real;
        base.Reset();
    }
    
    public override void PrimaryInteraction(Character character) {
        if (!isSabotaged && character.team == Team.Traitor && !Timer.SabotageTimer.IsStarted()) {
            Timer.SabotageTimer.Start(30);
            Sabotage();
            SetTaskDesc();
            view.RPC("AddTaskRPC", RpcTarget.All);
        } else if (isSabotaged && !(character.team == Team.Traitor)) {
            Fix();
            view.RPC("CompleteTask", RpcTarget.All);
            Timer.SabotageTimer.End();
            Reset();
        }
    }
    void Sabotage() {
        GetComponent<PhotonView>().RPC("SabotageRPC", RpcTarget.All);
    }

    [PunRPC]
	void SabotageRPC() {
      isSabotaged = true;
	}

    void Fix() {
        GetComponent<PhotonView>().RPC("FixRPC", RpcTarget.All);
    }

    [PunRPC]
	void FixRPC() {
      numberOfPlayersToFix--;  
      if (numberOfPlayersToFix == 0) isSabotaged = false;
	}

     void SetTaskDesc() {
        GetComponent<PhotonView>().RPC("SetTaskDescRPC", RpcTarget.All);
    }
     [PunRPC]
	void SetTaskDescRPC() {
      taskDescription = "Fix the " + this.name;
	}

}
