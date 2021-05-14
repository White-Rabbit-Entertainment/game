using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanRespawn : MonoBehaviour {
    [SerializeField] private GameSceneManager gameSceneManager;
    // Update is called once per frame
    void OnTriggerEnter(Collider collider) {
        if (collider.GetComponent<PhotonView>().IsMine) {

            // Get the collided thing, put it back on the map
            collider.transform.position = gameSceneManager.RandomNavmeshLocation();

            // If it has a rigid body stop it from moving
            if (collider.GetComponent<Rigidbody> != null) {
                collider.GetComponent<Rigidbody>.velocity = Vector3.Zero;
                collider.GetComponent<Rigidbody>.angularVelocity = Vector3.Zero;
            }
        }
    }
}
