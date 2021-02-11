using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class RobbersCaughtCounter : MonoBehaviour
{
    public Text counter;

    // Update is called once per frame
    void Update() {
        List<Player> players = NetworkManager.instance.GetPlayers();
        int count = 0;
        foreach (Player player in players) {
            if (NetworkManager.instance.PlayerPropertyIs("Captured", true, player)) {
                count++;
            }
        }
        counter.text = count.ToString();
    }
}

