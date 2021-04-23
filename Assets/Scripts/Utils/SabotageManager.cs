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

    public TextMeshProUGUI SabotageNotificationUI;
    public TextMeshProUGUI ClickAndHoldReminderUI;

    public GameObject SabotageTraitorCountdownUI;

    public TextMeshProUGUI SabotageTimeRemaining;
    public GameSceneManager gameSceneManager;

    public GameObject fixingProgress;

    public int numPlayersFixing = 0;

    public TextMeshProUGUI playersFixing;

    public float amountToFix;

    // Update is called once per frame
    void Update() {
        if (inSabotage) {
            SabotageTimeRemaining.text = $"{(int)Timer.SabotageTimer.TimeRemaining()}s";
            if (Timer.SabotageTimer.IsComplete()) {
                gameSceneManager.EndGame(Team.Traitor);

            }
            playersFixing.text = "Players Fixing: " + numPlayersFixing;
        }
    }

    public void SabotageStarted() {
        GetComponent<PhotonView>().RPC("SabotageStartedRPC", RpcTarget.All);
        
    }

    [PunRPC]
    public IEnumerator SabotageStartedRPC() {
        StartCoroutine(NotifySabotage());
        yield return new WaitForSeconds(5);
        numPlayersFixing = 0;
        Timer.SabotageTimer.Start(30);
        inSabotage = true;
        sabotageUI.SetActive(true);
        yield return new WaitForSeconds(7);
        warning.SetActive(false);
    }

    public IEnumerator NotifySabotage(){
        SabotageNotificationUI.text = "Sabotaged";
        SabotageTraitorCountdownUI.SetActive(true);
        Timer.TraitorSabotageTimer.Start(5);
        yield return new WaitForSeconds(5f);
        SabotageNotificationUI.text = "";
        SabotageTraitorCountdownUI.SetActive(false);
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
        ClickAndHoldReminderUI.text = "";
        isFixing = true;
        fixingProgress.SetActive(true);
    }

    public void LocalPlayerStoppedFixing() {
        isFixing = false;
        StartCoroutine(ClickAndHold());
        fixingProgress.SetActive(false);
        
    }

    public IEnumerator ClickAndHold(){
        ClickAndHoldReminderUI.text = "Click and hold to fix!";
        yield return new WaitForSeconds(2f);
        ClickAndHoldReminderUI.text = "";
    }

    public void SetAmountToFix(float amount) {
        GetComponent<PhotonView>().RPC("SetAmountToFixRPC", RpcTarget.All, amount);
    }

    [PunRPC]
    public void SetAmountToFixRPC(float amount) {
        this.amountToFix = amount;
    }

    public float GetAmountToFix(){
        return amountToFix; 
    }

    public bool GetInSabotage(){
        return inSabotage;
    }

    public bool GetIsFixing(){
        return isFixing;
    }

    public void SetNumPlayersFixing(int num) {
        GetComponent<PhotonView>().RPC("SetNumPlayersFixingRPC", RpcTarget.All, num);
    }

    [PunRPC]
    public void SetNumPlayersFixingRPC(int num) {
        this.numPlayersFixing = num;
    }
    public int GetNumPlayersFixing(){
        return numPlayersFixing;
    }
}
