using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableList : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        DontDestroyOnLoad(gameObject);
    }

    public void Freeze() {
        gameObject.SetActive(false);
    }
    
    public void Unfreeze() {
        gameObject.SetActive(true);
    }
}
