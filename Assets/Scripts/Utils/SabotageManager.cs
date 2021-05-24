using System.Collections;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// Handles logic and UI for sabotages
public class SabotageManager : MonoBehaviour{
    
    // UI
    [SerializeField] private GameObject sabotageUI;
    [SerializeField] private GameObject sabotageMiddleUI;
    [SerializeField] private GameObject taskUI;
    [SerializeField] private GameObject taskNotificationUI;
    [SerializeField] private Image colorOverlay;
    [SerializeField] private TextMeshProUGUI warningText;
    [SerializeField] private GameObject sabotageNotificationUI;
    [SerializeField] private TextMeshProUGUI clickAndHoldReminderUI;
    [SerializeField] private TextMeshProUGUI sabotageTimeRemaining;
    [SerializeField] private GameObject fixingProgress;
    [SerializeField] private TextMeshProUGUI playersFixing;
    [SerializeField] private TextMeshProUGUI sabotageInfoTMP;

    // Managers
    [SerializeField] private TimerManager timerManager;
    [SerializeField] private GameSceneManager gameSceneManager;

    // Music
    [SerializeField] private AudioSource sabotageMusic;
 
    // Current item being sabotaged
    public Sabotageable sabotageable;

    // Update is called once per frame
    void Update() {
        // If a sabotage is currently active check if the game has ended and update the UI
        if (sabotageable != null) {
            sabotageTimeRemaining.text = $"{(int)Timer.sabotageTimer.TimeRemaining()}s";

            // If the sabotage has not been completed in the time
            if (Timer.sabotageTimer.IsComplete()) {
                // End the game
                Timer.sabotageTimer.End();
                gameSceneManager.EndGame(Team.Traitor);

                sabotageMusic.Stop();
                // backgroundMusic.UnPause();
            }

            // Set ui to show number of players currently fixing the sabotageable
            playersFixing.text = "Players Fixing: " + sabotageable.numberOfPlayersFixing;
        }
    }

    public IEnumerator SabotageStarted(Sabotageable sabotageable) {
        this.sabotageable = sabotageable;
        StartCoroutine(NotifySabotage());

        // Wait till sabotage starts
        yield return new WaitForSeconds(5);

        // Start sabotage timer
        timerManager.StartTimer(Timer.sabotageTimer);

        // Play music
        sabotageMusic.Play();

        // Update UI
        sabotageMiddleUI.SetActive(true);
        taskUI.SetActive(false);
        taskNotificationUI.SetActive(false);
        warningText.gameObject.SetActive(true);
        sabotageUI.SetActive(true);

        // Set task marker
        PlayableCharacter me = NetworkManager.instance.GetMe();
        if (me is Loyal) {
            me.DisableTaskMarker();
        }

        yield return new WaitForSeconds(7);
        warningText.gameObject.SetActive(false);
    }

    // Adds overlay to screen to show sabotage is happening
    public void SetBackgroundImageColor(Sabotageable sabotageable){
        colorOverlay.color = sabotageable.color;
        warningText.text = sabotageable.warningText;
        sabotageInfoTMP.text = sabotageable.infoText;
    }

    // Notifies traitors that the sabotage has started
    public IEnumerator NotifySabotage(){
        if (NetworkManager.instance.GetMe() is Traitor){
            sabotageNotificationUI.SetActive(true);
            timerManager.StartTimer(Timer.traitorSabotageTimer);
            yield return new WaitForSeconds(5f);
            sabotageNotificationUI.SetActive(false);
        }
    }

    // When the sabotage is fixed updates ui, music and timer
    public void SabotageFixed() {

        // Update UI
        sabotageUI.SetActive(false);
        sabotageMiddleUI.SetActive(false);
        taskUI.SetActive(true);
        taskNotificationUI.SetActive(true);
        fixingProgress.SetActive(false);

        // End timer
        Timer.sabotageTimer.End();

        // Stop sabotage music
        sabotageMusic.Stop();
    }

    public void LocalPlayerStartedFixing() {
        clickAndHoldReminderUI.text = "";
        fixingProgress.SetActive(true);
    }

    public void LocalPlayerStoppedFixing() {
        StartCoroutine(ClickAndHold());
        fixingProgress.SetActive(false);
    }

    // Enable UI for fixing helper text
    public IEnumerator ClickAndHold(){
        clickAndHoldReminderUI.text = "Click and hold to fix!";
        yield return new WaitForSeconds(2f);
        clickAndHoldReminderUI.text = "";
    }
}
