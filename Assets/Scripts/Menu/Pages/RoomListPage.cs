using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomListPage : MenuPage {
    public Transform gridLayout;
    public GameObject roomNamePrefab;
 
    void OnEnable(){
        OnCachedRoomListUpdate(menuManager.roomList);
    }

    public void OnCachedRoomListUpdate(Dictionary<string, RoomInfo> roomList) {
        gridLayout.gameObject.DestroyChildren();
        foreach (RoomInfo room in roomList.Values) {
            Debug.Log($"Room: {room.Name}, Number of players: {room.PlayerCount}, IsVisible: {room.IsVisible}");
            if (room.IsVisible && room.PlayerCount > 0) {
                Debug.Log($"Found a visible room with some players");
                GameObject newRoom = Instantiate(roomNamePrefab, gridLayout.position, Quaternion.identity);
                newRoom.GetComponentInChildren<Text>().text = room.Name; //+ "(" + room.PlayerCount + ")";
                newRoom.transform.SetParent(gridLayout);
            }
        }
    }
}
