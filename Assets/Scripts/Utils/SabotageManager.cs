using System.Collections;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SabotageManager : MonoBehaviour
{
    public bool inSabotage = false;

    public bool isFixing = false;

    public GameObject sabotageUI;
    public GameObject warning;

    public TextMeshProUGUI SabotageTimeRemaining;
    public GameSceneManager gameSceneManager;

    public GameObject fixingProgress;

    public int numPlayersFixing;

    public TextMeshProUGUI playersFixing;

    public float amountToFix;

    // Update is called once per frame
    void Update() {
        if (inSabotage) {
            SabotageTimeRemaining.text = $"{(int)Timer.SabotageTimer.TimeRemaining()}s";
            if (Timer.SabotageTimer.IsComplete()) {
                gameSceneManager.EndGame(Team.Traitor);

            }
        }
    }

    public void SabotageStarted() {
        GetComponent<PhotonView>().RPC("SabotageStartedRPC", RpcTarget.All);
        
    }

    [PunRPC]
    public IEnumerator SabotageStartedRPC() {
        inSabotage = true;
        sabotageUI.SetActive(true);
        yield return new WaitForSeconds(7);
        warning.SetActive(false);
    }

    public void SabotageFixed() {
        GetComponent<PhotonView>().RPC("SabotageFixedRPC", RpcTarget.All);
    }

    [PunRPC]
    public void SabotageFixedRPC() {
        inSabotage = false;
        sabotageUI.SetActive(false);
        fixingProgress.SetActive(false);
    }

    public void LocalPlayerFixing() {
        fixingProgress.SetActive(true);
        playersFixing.text = "Players Fixing: " + numPlayersFixing;
    }

    public void LocalStopsFixing() {
        fixingProgress.SetActive(false);
    }

    public void SetAmountToFix(float amount) {
        GetComponent<PhotonView>().RPC("SetAmountToFixRPC", RpcTarget.All, amount);
    }

    [PunRPC]
    public void SetAmountToFixRPC(float amount) {
        amountToFix = amount;
    }

    public float GetAmountToFix(){
        return amountToFix; 
    }

    public bool GetInSabotage(){
        return inSabotage;
    }

    public void SetIsFixing(bool fixing){
        isFixing = fixing;
    }

    public bool GetIsFixing(){
        return isFixing;
    }

    public void SetNumPlayersFixing(int num){
        numPlayersFixing = num;
    }

    public int GetNumPlayersFixing(){
        return numPlayersFixing;
    }
}
