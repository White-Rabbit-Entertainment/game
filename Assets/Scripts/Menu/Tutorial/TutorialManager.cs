using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    public List<TutorialStage> stages; 
    public int currentStage;

    // Start is called before the first frame update
    void Start()
    {
       currentStage = 0;
    }

    public void NextStage() {
        stages[currentStage].gameObject.SetActive(false);
        if (currentStage < stages.Count) {
            currentStage++;
            stages[currentStage].gameObject.SetActive(true);
        } else {
            EndTutorial();
        }
    }

    public void EndTutorial() {
        Debug.Log("Tutorial ended");
    }

    // Update is called once per frame
    void Update()
    {
        // Advance to next stage
        if (Input.GetKeyDown(KeyCode.P)) {
            NextStage();
        }
       
        // Skip
        if (Input.GetKeyDown(KeyCode.S)) {
            EndTutorial();
        }
    }
}
