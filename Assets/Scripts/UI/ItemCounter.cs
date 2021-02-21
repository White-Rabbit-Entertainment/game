using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class ItemCounter : MonoBehaviour {
    public Text counter;
    void Update() {
        // if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("ItemsStolen")) {
        //     counter.text = PhotonNetwork.CurrentRoom.CustomProperties["ItemsStolen"].ToString();
        // }
        counter.text = NetworkManager.instance.GetRoomProperty<int>("ItemsStolen", 0).ToString();
    }
}
