using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class PlayerSpawner : MonoBehaviour
{

    public GameObject robberPrefab;
    public GameObject seekerPrefab;
    // Start is called before the first frame update
    void Start() {
        foreach(var key in PhotonNetwork.LocalPlayer.CustomProperties.Keys) {
            Debug.Log(String.Format("{0}: {1}", key, PhotonNetwork.LocalPlayer.CustomProperties[key]));
        }
        GameObject playerPrefab = seekerPrefab;
        if (PhotonNetwork.LocalPlayer.NickName == "seeker") {
            playerPrefab = seekerPrefab;
        } else if (PhotonNetwork.LocalPlayer.NickName == "robber") {
            playerPrefab = robberPrefab;
        }
        PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(1,2,-10), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
