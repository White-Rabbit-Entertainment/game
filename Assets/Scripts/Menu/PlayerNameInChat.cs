using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerNameInChat : MonoBehaviour
{       
    Text text;
    void Start()
    {
        text = GameObject.Find("localPlayerName").GetComponent<Text>();
        text.text = PhotonNetwork.LocalPlayer.NickName;
    }

}
