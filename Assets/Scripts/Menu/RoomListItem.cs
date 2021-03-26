using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


public class RoomListItem : MonoBehaviour {
    [SerializeField] Text text;

    public string playerName;
    public RoomInfo info;

    public void Start() {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnClick() {
        NetworkManager.instance.JoinRoom(text.text, playerName);   
    }
}
