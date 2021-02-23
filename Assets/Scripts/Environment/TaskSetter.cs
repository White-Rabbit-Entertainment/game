using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class TaskSetter : MonoBehaviour {

    public GameObject taskables;
    public GameObject stealables;

    void Update() {
        if (NetworkManager.instance.AllPlayersInGame()) {
          CreateTasks();
          Destroy(this);
        }
    }

    void CreateTasks() {
        if (PhotonNetwork.LocalPlayer.IsMasterClient) {
            // Get the number of tasks of each type which should be created
            int numberOfNonStealingTasks = NetworkManager.instance.GetRoomProperty<int>("NumberOfNonStealingTasks");
            int numberOfStealingTasks = NetworkManager.instance.GetRoomProperty<int>("NumberOfStealingTasks");

            // Get all possible items to assign tasks to in the environment 
            // We split this so we can assign the correct number of stealing
            // tasks.
            List<Transform> possibleTaskables = new List<Transform>();
            List<Transform> possibleStealables = new List<Transform>();
            foreach(Transform transform in taskables.transform) {
                possibleTaskables.Add(transform);
            }
            foreach(Transform transform in stealables.transform) {
                possibleStealables.Add(transform);
            }

            // Shuffle those lists randomly 
            Utils.Shuffle<Transform>(possibleStealables);
            Utils.Shuffle<Transform>(possibleTaskables);

            // Assign the first few itmes a Task
            for (int i = 0; i < numberOfNonStealingTasks; i++) {
                PhotonView view = possibleTaskables[i].GetComponent<PhotonView>();
                view.RPC("AddTask", RpcTarget.All);
            }
            for (int i = 0; i < numberOfStealingTasks; i++) {
                PhotonView view = possibleStealables[i].GetComponent<PhotonView>();
                view.RPC("AddTask", RpcTarget.All);
            }

            // Say that we have finished the work of setting up tasks (used for
            // knowing everything is loaded).
            NetworkManager.instance.SetRoomProperty("TasksSet", true);
        }
    }
}