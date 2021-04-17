using System;
using UnityEngine;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class WebRTC : MonoBehaviour {

    [DllImport("__Internal")]
    private static extern void Init();
    
    [DllImport("__Internal")]
    private static extern string MakeOffer();
    
    [DllImport("__Internal")]
    private static extern string MakeAnswer(string sdp, int callerId);
    
    [DllImport("__Internal")]
    private static extern string ApplyAnswer(string sdp);
    // 
    // [DllImport("__Internal")]
    // private static extern string ApplyIceCandidate(string candidateData);
    
    [DllImport("__Internal")]
    private static extern void HelloString(string str);

    PhotonView View {
        get {return GetComponent<PhotonView>();}
    }

    public void InitWebRTC() {
        Init();
        string sdp = MakeOffer();
        HelloString(sdp);
    }

    public void Working(string thing) {
        HelloString(thing);
    }

    public void SendOffer(string sdp) {
        View.RPC("HandleOffer", RpcTarget.OthersBuffered, sdp, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    [PunRPC]
    public void HandleOffer(string sdp, int callerId) {
        Debug.Log("Offer received");
        MakeAnswer(sdp, callerId);
    }

    public void SendAnswer(string sdp, int callerId) {
        View.RPC("HandleAnswer", PhotonNetwork.LocalPlayer.Get(callerId), sdp);
    }
    
    [PunRPC]
    public void HandleAnswer(string sdp) {
        ApplyAnswer(sdp);
    }
    
    public void OnConnected() {
        Debug.Log("Connected!");
        HelloString("Connected");
    }

    public void SendIceCandidate(string candidate) {
        View.RPC("HandleIceCandidate", RpcTarget.OthersBuffered, candidate);
    }

    [PunRPC]
    public void HandleIceCandidate(string candidateData) {
    //    ApplyIceCandidate(candidateData);
    }
}

