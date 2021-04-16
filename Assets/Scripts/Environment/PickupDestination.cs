using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupDestination : MonoBehaviour {
    public void EnableTaskMarker() {
        GetComponent<Target>().enabled = true;
    }
    
    public void DisableTaskMarker() {
        GetComponent<Target>().enabled = false;
    }
}
