using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class StealTaskSetter : MonoBehaviour {

    public GameObject stealables;
    private List<Transform> possibleStealables;
    
    void Update() {
        if (NetworkManager.instance.AllPlayersInGame()) {
          InitItems();
          Destroy(this);
        }
    }

    void InitItems() {
        if (PhotonNetwork.LocalPlayer.IsMasterClient) {
            int numberOfTargetItems = NetworkManager.instance.GetRoomProperty<int>("NumberOfTargetItems");
            possibleStealables = new List<Transform>();
            foreach(Transform transform in stealables.transform) {
                possibleStealables.Add(transform);
            }

            Utils.Shuffle<Transform>(possibleStealables);
            for (int i = 0; i < numberOfTargetItems; i++) {
                PhotonView view = possibleStealables[i].GetComponent<PhotonView>();
                view.RPC("MakeStealable", RpcTarget.All);
                view.RPC("AddTask", RpcTarget.All);
            }
        }
    }

    
}
