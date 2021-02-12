using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicItem : MonoBehaviour
{
    public void Pickup(Transform pickupDestination) {
        gameObject.tag = "Interactable";
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<Rigidbody>().useGravity = false;
        transform.position = pickupDestination.position;
        transform.parent = pickupDestination;
    }

    public void Drop() {
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
