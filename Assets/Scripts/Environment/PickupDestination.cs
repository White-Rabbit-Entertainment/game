using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupDestination : MonoBehaviour {

    [SerializeField] public string name;
    [SerializeField] private GameObject destinationZoneIndicator;

    public void EnableTaskMarker() {
        EnableDestinationZone();
        GetComponent<Target>().enabled = true;
    }
    
    public void DisableTaskMarker() {
        DisableDestinationZone();
        GetComponent<Target>().enabled = false;
    }

    public void EnableDestinationZone() {
        destinationZoneIndicator.SetActive(true);
    }
    
    public void DisableDestinationZone() {
        destinationZoneIndicator.SetActive(false);
    }
}
