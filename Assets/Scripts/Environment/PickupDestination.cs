using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupDestination : MonoBehaviour {
    public void EnableTarget() {
        GetComponent<Target>().enabled = true;
    }
    
    public void DisableTarget() {
        GetComponent<Target>().enabled = false;
    }
}
