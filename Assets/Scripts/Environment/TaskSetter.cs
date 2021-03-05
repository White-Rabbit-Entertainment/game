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
            List<Transform> possibleMasterTaskables = new List<Transform>();
            List<Transform> possibleTaskables = new List<Transform>();
            List<Transform> possibleStealables = new List<Transform>();
            foreach(Transform transform in taskables.transform) {
                possibleTaskables.Add(transform);
                if (transform.GetComponent<Interactable>().canBeMasterTask) {
                    possibleMasterTaskables.Add(transform);
                }
         
            }
            foreach(Transform transform in stealables.transform) {
                possibleStealables.Add(transform);
            }

            // Shuffle those lists randomly 
            Utils.Shuffle<Transform>(possibleStealables);
            Utils.Shuffle<Transform>(possibleMasterTaskables);

            // Assign the first few itmes a Task
            for (int i = 0; i < numberOfNonStealingTasks; i++) {
                PhotonView view = possibleMasterTaskables[i].GetComponent<PhotonView>();
                if (possibleMasterTaskables[i].GetComponent<Interactable>().HasSoftRequirements()) {
                    Debug.Log("the item we are adding a task to has soft requirements");
                    possibleMasterTaskables[i].GetComponent<Interactable>().PickHardRequirement(possibleTaskables);
                }
                PhotonView view = possibleTaskables[i].GetComponent<PhotonView>();
                view.RPC("AddTaskRPC", RpcTarget.All);
            }
            for (int i = 0; i < numberOfStealingTasks; i++) {
                PhotonView view = possibleStealables[i].GetComponent<PhotonView>();
                view.RPC("AddTaskRPC", RpcTarget.All);
            }

            // Say that we have finished the work of setting up tasks (used for
            // knowing everything is loaded).
            NetworkManager.instance.SetRoomProperty("TasksSet", true);
        }
    }
}
