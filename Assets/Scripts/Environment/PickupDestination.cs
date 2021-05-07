using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupDestination : MonoBehaviour {

    [SerializeField] public string name;

    public void EnableTaskMarker() {
        gameObject.SetActive(true);
        GetComponent<Target>().enabled = true;
    }
    
    public void DisableTaskMarker() {
        gameObject.SetActive(false);
        GetComponent<Target>().enabled = false;
    }
}
