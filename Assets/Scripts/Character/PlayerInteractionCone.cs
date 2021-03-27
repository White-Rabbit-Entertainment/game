using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionCone : MonoBehaviour
{

    ItemInteract itemInteract;

    void Start() {
        itemInteract = transform.parent.parent.GetComponent<ItemInteract>();
    }
    
    void OnTriggerEnter(Collider collider) {
        itemInteract.OnInteracitonConeEnter(collider); // pass the own collider and the one we've hit
    }
    
    void OnTriggerExit(Collider collider) {
        itemInteract.OnInteracitonConeExit(collider); // pass the own collider and the one we've hit
    }
}
