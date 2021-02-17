using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour {

    public GameObject seekerPrefab;
    public GameObject robberPrefab;
    
    void Awake() {
        if (NetworkManager.instance.LocalPlayerPropertyIs<string>("Team", "Seeker")) {
            PhotonNetwork.Instantiate(seekerPrefab.name, new Vector3(1,2,-10), Quaternion.identity);
        } else if (NetworkManager.instance.LocalPlayerPropertyIs<string>("Team", "Robber")) {
            PhotonNetwork.Instantiate(robberPrefab.name, new Vector3(1,2,-10), Quaternion.identity);
        } else {
            Debug.Log("no team");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
