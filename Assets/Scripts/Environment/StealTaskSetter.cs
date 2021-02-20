using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class StealTaskSetter : MonoBehaviour {

    public GameObject stealables;
    public StealingTask[] stealingTasks;

    void LoadItems() {
        // foreach (GameObject spawn in itemSpawns.transform) {
        //     Debug.Log("spawn found");
        // }
    }

    void Start() {
        if (PhotonNetwork.LocalPlayer.IsMasterClient) {
            int numberOfTargetItems = NetworkManager.instance.GetRoomProperty<int>("NumberOfTargetItems");
            StealingTask[] tasks = new StealingTask[numberOfTargetItems];
            List<Transform> possibleStealables = new List<Transform>();
            foreach(Transform transform in stealables.transform) {
                possibleStealables.Add(transform);
            }
            Utils.Shuffle<Transform>(possibleStealables);
            for (int i = 0; i < numberOfTargetItems; i++) {
                possibleStealables[i].gameObject.AddComponent<StealableItem>();
                tasks[i] = new StealingTask("Steal item", possibleStealables[i]);
            }
            stealingTasks = tasks;
    }
    }
    
}
