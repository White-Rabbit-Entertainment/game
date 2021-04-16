using System;
using UnityEngine;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;
using Photon.Pun;

public class WebRTC : MonoBehaviour {

    [DllImport("__Internal")]
    private static extern void Init();
    
    [DllImport("__Internal")]
    private static extern string MakeOffer();
    
    [DllImport("__Internal")]
    private static extern string MakeAnswer(string sdp);
    
    [DllImport("__Internal")]
    private static extern void HelloString(string str);

    PhotonView View {
        get {return GetComponent<PhotonView>();}
    }

    void Start() {
        Init();
        string sdp = MakeOffer();
        HelloString(sdp);
        
    }

    public void Working(string thing) {
        HelloString(thing);
    }

    public void SendOffer(string sdp) {
        View.RPC("HandleOffer", RpcTarget.Others, sdp);
    }

    [PunRPC]
    public void HandleOffer(string sdp) {
        Debug.Log("Offer received");
        MakeAnswer(sdp);
    }

    public void SendAnswer(string sdp) {
    }
}

