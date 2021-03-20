using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class TaskSetter : MonoBehaviour {

    public GameObject taskables;

    void Update() {
        if (NetworkManager.instance.AllPlayersInGame() && NetworkManager.instance.AllCharactersSpawned()) {
          CreateTasks();
          Destroy(this);
        }
    }

    void CreateTasks() {
        if (PhotonNetwork.LocalPlayer.IsMasterClient) {
            // Get the number of tasks of each type which should be created
            int numberOfTasks = NetworkManager.instance.GetRoomProperty<int>("NumberOfTasks");
            // Get all possible items to assign tasks to in the environment 
            // We split this so we can assign the correct number of stealing
            // tasks.
            List<Transform> possibleMasterTaskables = new List<Transform>();
            List<Transform> possibleTaskables = new List<Transform>();

            // Creates a list of all item which can be assigned a task and
            // seperate list for those which can be master tasks
            foreach(Transform transform in taskables.transform) {
                possibleTaskables.Add(transform);
                if (transform.GetComponent<Interactable>().canBeMasterTask) {
                    possibleMasterTaskables.Add(transform);
                }
         
            }

            // Shuffle those lists randomly 
            possibleMasterTaskables.Shuffle();

            // Assign the first few items a Task
            for (int i = 0; i < numberOfTasks; i++) {
                if (possibleMasterTaskables[i].GetComponent<Interactable>().HasSoftRequirements()) {
                    int numberOfSubTasks = Random.Range(0, 2);
                    if (numberOfSubTasks > 0) {
                         possibleMasterTaskables[i].GetComponent<Interactable>().PickHardRequirements(possibleTaskables);
                    }
                }
                PhotonView view = possibleMasterTaskables[i].GetComponent<PhotonView>();
                view.RPC("AddTaskRPC", RpcTarget.All);
            }
            // Say that we have finished the work of setting up tasks (used for
            // knowing everything is loaded).
            NetworkManager.instance.SetRoomProperty("TasksSet", true);
        }
    }
}
