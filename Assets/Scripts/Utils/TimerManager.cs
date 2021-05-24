using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// Handles Photon syncrhoniation for timers
public class TimerManager : MonoBehaviour {

    private PhotonView View {
        get { return GetComponent<PhotonView>();}
    }

    // Start a timer for all players
    public void StartTimer(Timer timer) {
        View.RPC("StartTimerRPC", RpcTarget.All, timer.id, PhotonNetwork.Time);
    } 
    
    // End a timer for all players
    public void EndTimer(Timer timer) {
        View.RPC("EndTimerRPC", RpcTarget.All, timer.id);
    } 

    [PunRPC] 
    public void StartTimerRPC(string timerId, double startTime) {
        Timer timer = Timer.Get(timerId);
        timer.Start(startTime);
    }
    
    [PunRPC] 
    public void EndTimerRPC(string timerId) {
        Timer timer = Timer.Get(timerId);
        timer.End();
    }
}
