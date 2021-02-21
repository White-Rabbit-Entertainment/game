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
            int numberOfNonStealingTasks = NetworkManager.instance.GetRoomProperty<int>("NumberOfNonStealingTasks");
            int numberOfStealingTasks = NetworkManager.instance.GetRoomProperty<int>("NumberOfStealingTasks");
            List<Transform> possibleTaskables = new List<Transform>();
            List<Transform> possibleStealables = new List<Transform>();

            foreach(Transform transform in taskables.transform) {
                possibleTaskables.Add(transform);
            }
            foreach(Transform transform in stealables.transform) {
                possibleStealables.Add(transform);
            }

            Utils.Shuffle<Transform>(possibleStealables);
            Utils.Shuffle<Transform>(possibleTaskables);

            for (int i = 0; i < numberOfNonStealingTasks; i++) {
                PhotonView view = possibleTaskables[i].GetComponent<PhotonView>();
                view.RPC("AddTask", RpcTarget.All);
            }
            for (int i = 0; i < numberOfStealingTasks; i++) {
                PhotonView view = possibleStealables[i].GetComponent<PhotonView>();
                view.RPC("AddTask", RpcTarget.All);
            }

            NetworkManager.instance.SetRoomProperty("TasksSet", true);
        }
    }
}
