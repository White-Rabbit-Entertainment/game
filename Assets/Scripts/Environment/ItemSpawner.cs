using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class ItemSpawner : MonoBehaviour {

    public GameObject itemPrefab;
    public GameObject itemSpawns;

    void LoadItems() {
        foreach (GameObject spawn in itemSpawns.transform) {
            Debug.Log("spawn found");
        }
    }

    void Update() {
        if (NetworkManager.instance.AllPlayersInGame()) {
          LoadItems();
          Destroy(this);
          Destroy(gameObject);
        }
    }
}
