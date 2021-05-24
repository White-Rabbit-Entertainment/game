using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// When an item hits the ocean (i.e. thrown off the map) it should be respawned
// at a random spawn point on this ship.
public class OceanRespawn : MonoBehaviour {
    [SerializeField] private GameSceneManager gameSceneManager;
    // Update is called once per frame
    void OnTriggerEnter(Collider collider) {
        Debug.Log($"Something kinda hit the floor: {collider.gameObject}");
        if (collider.GetComponent<PhotonView>() != null && collider.GetComponent<PhotonView>().IsMine) {

            Debug.Log($"Something hit floor: {collider.gameObject}");
            // Get the collided thing, put it back on the map
            collider.transform.position = gameSceneManager.RandomNavmeshLocation();

            // If it has a rigid body stop it from moving
            if (collider.GetComponent<Rigidbody>() != null) {
                collider.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
                collider.GetComponent<Rigidbody>().angularVelocity = new Vector3(0,0,0);
            }
        }
    }
}
