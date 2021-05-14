using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanRespawn : MonoBehaviour {
    // Update is called once per frame
    void OnTriggerEnter(Collider collider) {
       Debug.Log($"Something hit the sea, {collider.gameObject}");
    }
}
