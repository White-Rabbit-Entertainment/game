using System.Collections;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SabotageManager : MonoBehaviour{
    private bool inSabotage = false;
    private int numPlayersFixing = 0;
    private bool isFixing = false;

    [SerializeField] private  GameObject sabotageUI;
    [SerializeField] private  GameObject taskUI;

    [SerializeField] private Image colorOverlay;
    [SerializeField] private TextMeshProUGUI warningText;
                             
    [SerializeField] private GameObject sabotageNotificationUI;
    [SerializeField] private TextMeshProUGUI clickAndHoldReminderUI;
    
    [SerializeField] private TextMeshProUGUI sabotageTimeRemaining;
    [SerializeField] private GameSceneManager gameSceneManager;
                             
    [SerializeField] private GameObject fixingProgress;
    [SerializeField] private TextMeshProUGUI playersFixing;
                             
    [SerializeField] private TextMeshProUGUI sabotageInfoTMP;
                             
    [SerializeField] private TimerManager timerManager;
                             
    public float amountToFix;

    // Update is called once per frame
    void Update() {
        if (inSabotage) {
            sabotageTimeRemaining.text = $"{(int)Timer.sabotageTimer.TimeRemaining()}s";

            // If the sabotage has not been completed in the time
            if (Timer.sabotageTimer.IsComplete()) {
                // End the game
                Timer.sabotageTimer.End();
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
        // Wait till sabotage starts
        yield return new WaitForSeconds(5);
        numPlayersFixing = 0;
        timerManager.StartTimer(Timer.sabotageTimer);
        inSabotage = true;
        warningText.gameObject.SetActive(true);
        sabotageUI.SetActive(true);
        taskUI.SetActive(false);

        PlayableCharacter me = NetworkManager.instance.GetMe();
        if (me is Loyal) {
            me.DisableTaskMarker();
        }

        yield return new WaitForSeconds(7);
        warningText.gameObject.SetActive(false);
    }

    public void SetBackgroundImageColor(Sabotageable sabotageable){
        colorOverlay.color = sabotageable.color;
        warningText.text = sabotageable.warningText;
        sabotageInfoTMP.text = sabotageable.infoText;
    }

    public IEnumerator NotifySabotage(){
        if (NetworkManager.instance.GetMe() is Traitor){
            sabotageNotificationUI.SetActive(true);
            timerManager.StartTimer(Timer.traitorSabotageTimer);
            yield return new WaitForSeconds(5f);
            sabotageNotificationUI.SetActive(false);
        }
    }

    public void SabotageFixed() {
        inSabotage = false;
        sabotageUI.SetActive(false);
        taskUI.SetActive(true);
        fixingProgress.SetActive(false);
        Timer.sabotageTimer.End();
    }

    public void LocalPlayerFixing() {
        clickAndHoldReminderUI.text = "";
        isFixing = true;
        fixingProgress.SetActive(true);
    }

    public void LocalPlayerStoppedFixing() {
        isFixing = false;
        StartCoroutine(ClickAndHold());
        fixingProgress.SetActive(false);
        
    }

    public IEnumerator ClickAndHold(){
        clickAndHoldReminderUI.text = "Click and hold to fix!";
        yield return new WaitForSeconds(2f);
        clickAndHoldReminderUI.text = "";
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
        numPlayersFixing = num;
    }
    public int GetNumPlayersFixing(){
        return numPlayersFixing;
    }
}
