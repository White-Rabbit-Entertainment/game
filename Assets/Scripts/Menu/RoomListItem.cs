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

    // public void Start() {
    //     info = _info;
    //     text.text = _info.Name;
    // }

    public void Start() {
        Debug.Log("Start");
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnClick() {
        Debug.Log("Click");
        NetworkManager.instance.JoinRoom(text.text, playerName);   
    }
}
