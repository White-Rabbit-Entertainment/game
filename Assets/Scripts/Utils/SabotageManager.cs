using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SabotageManager : MonoBehaviour
{
    private bool inSabotage = false;

    public GameSceneManager gameSceneManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (inSabotage) {
    //   voteTimeRemaining.text = $"{(int)Timer.VoteTimer.TimeRemaining()}s";
      if (Timer.SabotageTimer.IsComplete()) {
        gameSceneManager.EndGame(Team.Traitor);

      }
    }
    }

    public void SabotageStarted() {
        GetComponent<PhotonView>().RPC("SabotageStartedRPC", RpcTarget.All);
    }

    [PunRPC]
    public void SabotageStartedRPC() {
        inSabotage = true;
    }

    public void SabotageFixed() {
        GetComponent<PhotonView>().RPC("SabotageFixedRPC", RpcTarget.All);
    }

    [PunRPC]
    public void SabotageFixedRPC() {
        inSabotage = false;
    }

    public void LocalPlayerFixing() {

    }

    public void LocalStopsFixing() {
        
    }

}
