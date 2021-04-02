using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour {

    public List<TutorialStage> stages; 
    public int currentStage;
    public Task tutorialTask;

    // Start is called before the first frame update
    void Start() {
       currentStage = 0;
    }

    public void NextStage() {
        stages[currentStage].gameObject.SetActive(false);
        if (currentStage < stages.Count) {
            currentStage++;
            stages[currentStage].gameObject.SetActive(true);
            if (currentStage == 1) {
                tutorialTask.GetComponent<Interactable>().EnableTarget();
            } 

        } else {
            EndTutorial();
        }
    }

    public void EndTutorial() {
        NetworkManager.instance.ChangeScene("LobbyScene");
        Debug.Log("Tutorial ended");
    }

    // Update is called once per frame
    void Update() {
        // Advance to next stage
        if (Input.GetKeyDown(KeyCode.L)) {
            NextStage();
        }
       
        // Skip
        if (Input.GetKeyDown(KeyCode.K)) {
            EndTutorial();
        }
    }
}
