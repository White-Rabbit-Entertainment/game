using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionCone : MonoBehaviour {

    ItemInteract itemInteract;

    //Get item interact component from character
    void Start() {
        itemInteract = transform.parent.parent.GetComponent<ItemInteract>();
    }
    
    //Handle object entering trigger collider
    void OnTriggerEnter(Collider collider) {
        itemInteract.OnInteractionConeEnter(collider);
    }
    
    //Handle object leaving trigger collider
    void OnTriggerExit(Collider collider) {
        itemInteract.OnInteractionConeExit(collider);
    }
}
