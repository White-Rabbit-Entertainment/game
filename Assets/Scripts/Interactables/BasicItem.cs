using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicItem : MonoBehaviour, Interactable {
    public void Pickup(Transform pickupDestination) {
        
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<Rigidbody>().useGravity = false;
        transform.position = pickupDestination.position;
        transform.parent = pickupDestination;
    }

    public void Drop() {
    }

    // Start is called before the first frame update
    void Start() {
        gameObject.tag = "interactable";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
