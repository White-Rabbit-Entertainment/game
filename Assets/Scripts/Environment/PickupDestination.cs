using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupDestination : MonoBehaviour {

    [SerializeField] public string name;
    [SerializeField] private GameObject destinationZoneIndicator;

    [SerializeField] List<PickupDestinationExtension> pickupDestinationExtensions;

    public void EnableTaskMarker() {
        EnableDestinationZone();
        GetComponent<Target>().enabled = true;
    }
    
    public void DisableTaskMarker() {
        DisableDestinationZone();
        GetComponent<Target>().enabled = false;
    }

    public void EnableDestinationZone() {
        if (destinationZoneIndicator != null) {
            destinationZoneIndicator.SetActive(true);
        }
    }
    
    public void DisableDestinationZone() {
        if (destinationZoneIndicator != null) {
            destinationZoneIndicator.SetActive(false);
        }
    }

    public bool IsPartOfPickUpDestination(GameObject queryGameObject) {
        if (queryGameObject == gameObject) return true;
        foreach(PickupDestinationExtension extension in pickupDestinationExtensions) {
            if (queryGameObject == extension.gameObject) return true;
        }
        return false;
    }
}
