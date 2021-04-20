using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class SabotageManager : MonoBehaviour
{
    private bool inSabotage = false;

    public TextMeshProUGUI SabotageTimeRemaining;
    public GameSceneManager gameSceneManager;

    private float amountToFix;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (inSabotage) {
            Debug.Log(amountToFix);
            // SabotageTimeRemaining.text = $"{(int)Timer.SabotageTimer.TimeRemaining()}s";
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

    public void SetAmountToFix(float amount) {
        GetComponent<PhotonView>().RPC("SabotageStartedRPC", RpcTarget.All);
    }

    [PunRPC]
    public void SetAmountToFixRPC(float amount) {
        amountToFix = amount;
    }

}
