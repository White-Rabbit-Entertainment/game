using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour {

    public GameObject seekerPrefab;
    public GameObject robberPrefab;

    private bool playerIsLoaded = false;

    void Start() {
      
    }
    void OnEnable() {
    //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable() {
    //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
        NetworkManager.instance.SetLocalPlayerProperty("InGameScene", true);
        // while(!NetworkManager.instance.AllPlayersInGame()) {
        //     Debug.Log("not all players in scene");
        // }
        // if (NetworkManager.instance.LocalPlayerPropertyIs<string>("Team", "Seeker")) {
        //     PhotonNetwork.Instantiate(seekerPrefab.name, new Vector3(1,2,-10), Quaternion.identity);
        // } else if (NetworkManager.instance.LocalPlayerPropertyIs<string>("Team", "Robber")) {
        //     PhotonNetwork.Instantiate(robberPrefab.name, new Vector3(1,2,-10), Quaternion.identity);
        // } else {
        //     Debug.Log("no team");
        // }
    }

    void LoadPlayer() {
        if (!playerIsLoaded) {
            playerIsLoaded = true;
            if (NetworkManager.instance.LocalPlayerPropertyIs<string>("Team", "Seeker")) {
                Debug.Log("Instantiating seeker");
                PhotonNetwork.Instantiate(seekerPrefab.name, new Vector3(1,2,-10), Quaternion.identity);
            } else if (NetworkManager.instance.LocalPlayerPropertyIs<string>("Team", "Robber")) {
                Debug.Log("Instantiating robber");
                PhotonNetwork.Instantiate(robberPrefab.name, new Vector3(1,2,-10), Quaternion.identity);
            }
        }
    }

    void Update() {
        if (NetworkManager.instance.AllPlayersInGame()) {
            LoadPlayer();
            Destroy(this);
        }
    }
}
