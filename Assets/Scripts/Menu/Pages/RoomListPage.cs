using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomListPage : MenuPage {
    public Transform gridLayout;
    public GameObject roomNamePrefab;
 
    void Start(){
        OnRoomListUpdate(menuManager.roomList);
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList) {

        gridLayout.gameObject.DestroyChildren();
        foreach (var room in roomList) {
            if (room.IsVisible && room.PlayerCount > 0) {
                GameObject newRoom = Instantiate(roomNamePrefab, gridLayout.position, Quaternion.identity);
                newRoom.GetComponentInChildren<Text>().text = room.Name; //+ "(" + room.PlayerCount + ")";
                newRoom.transform.SetParent(gridLayout);
            }
        }
    }
}
