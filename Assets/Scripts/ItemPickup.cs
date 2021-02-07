using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Transform pickupDestination;

    void OnMouseDown() {
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<Rigidbody>().useGravity = false;
        transform.position = pickupDestination.position;
        transform.parent = GameObject.Find("Item Pickup Destination").transform;
    }

    void OnMouseUp() {
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<Rigidbody>().useGravity = true;
        transform.parent = null;
    }
}
