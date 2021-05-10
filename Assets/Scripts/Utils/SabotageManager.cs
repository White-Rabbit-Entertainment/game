using System.Collections;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SabotageManager : MonoBehaviour{
    [SerializeField] private  GameObject sabotageUI;
    [SerializeField] private GameObject sabotageMiddleUI;
    [SerializeField] private  GameObject taskUI;

    [SerializeField] private GameObject taskNotificationUI;

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

  
                             
    public Sabotageable sabotageable;

    // Update is called once per frame
    void Update() {
        if (sabotageable != null) {
            sabotageTimeRemaining.text = $"{(int)Timer.sabotageTimer.TimeRemaining()}s";

            // If the sabotage has not been completed in the time
            if (Timer.sabotageTimer.IsComplete()) {
                // End the game
                Timer.sabotageTimer.End();
                sabotageable.Fix();
                gameSceneManager.EndGame(Team.Traitor);
            }
            playersFixing.text = "Players Fixing: " + sabotageable.numberOfPlayersFixing;
        }
    }

    public IEnumerator SabotageStarted(Sabotageable sabotageable) {
        this.sabotageable = sabotageable;
        StartCoroutine(NotifySabotage());
        // Wait till sabotage starts
        yield return new WaitForSeconds(5);
        timerManager.StartTimer(Timer.sabotageTimer);
        warningText.gameObject.SetActive(true);
        sabotageUI.SetActive(true);
        // if (NetworkManager.instance.GetMe() is Loyal){
        //     sabotageMiddleUI.SetActive(true);
        // }
        sabotageMiddleUI.SetActive(true);
        taskUI.SetActive(false);
        taskNotificationUI.SetActive(false);
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
        sabotageUI.SetActive(false);
        sabotageMiddleUI.SetActive(false);
        taskUI.SetActive(true);
        taskNotificationUI.SetActive(true);
        fixingProgress.SetActive(false);
        Timer.sabotageTimer.End();
    }

    public void LocalPlayerStartedFixing() {
        clickAndHoldReminderUI.text = "";
        fixingProgress.SetActive(true);
    }

    public void LocalPlayerStoppedFixing() {
        StartCoroutine(ClickAndHold());
        fixingProgress.SetActive(false);
    }

    public IEnumerator ClickAndHold(){
        clickAndHoldReminderUI.text = "Click and hold to fix!";
        yield return new WaitForSeconds(2f);
        clickAndHoldReminderUI.text = "";
    }
}
