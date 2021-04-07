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
        for (int i = 0; i < gridLayout.childCount; i++) {
            if (gridLayout.GetChild(i).gameObject.GetComponentInChildren<Text>().text == roomList[i].Name) {
                Destroy(gridLayout.GetChild(i).gameObject);

                if (roomList[i].PlayerCount == 0) {
                    roomList.Remove(roomList[i]);
                }
            }
        }

        foreach (var room in roomList) {
            if (room.Name.StartsWith("p")) {
                continue;
            }
            
            GameObject newRoom = Instantiate(roomNamePrefab, gridLayout.position, Quaternion.identity);
            newRoom.GetComponentInChildren<Text>().text = room.Name; //+ "(" + room.PlayerCount + ")";
            newRoom.transform.SetParent(gridLayout);
        }
    }
}
