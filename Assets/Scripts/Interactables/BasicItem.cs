﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BasicItem : MonoBehaviour, Interactable {

    public void PickUp(Transform pickupDestination) {
        GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        transform.position = pickupDestination.position;
        transform.parent = pickupDestination;
    }

    public void PutDown() {
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        transform.parent = GameObject.Find("/Environment").transform;
    }

    // Start is called before the first frame update
    void Start() {
        gameObject.tag = "interactable";
    }
}
