using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour {

    public GameObject seekerPrefab;
    public GameObject robberPrefab;

    void OnEnable() {
    //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable() {
    //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
        if (NetworkManager.instance.LocalPlayerPropertyIs<string>("Team", "Seeker")) {
            PhotonNetwork.Instantiate(seekerPrefab.name, new Vector3(1,2,-10), Quaternion.identity);
        } else if (NetworkManager.instance.LocalPlayerPropertyIs<string>("Team", "Robber")) {
            PhotonNetwork.Instantiate(robberPrefab.name, new Vector3(1,2,-10), Quaternion.identity);
        } else {
            Debug.Log("no team");
        }
    }
}
