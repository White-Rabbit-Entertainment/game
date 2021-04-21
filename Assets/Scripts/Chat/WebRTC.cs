using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class WebRTC : MonoBehaviour {

    [DllImport("__Internal")]
    private static extern void Init();
    
    [DllImport("__Internal")]
    private static extern void MakeOffer();
    
    [DllImport("__Internal")]
    private static extern void MakeAnswer(string sdp, int callerId);
    
    [DllImport("__Internal")]
    private static extern void ApplyAnswer(string sdp);
     
    [DllImport("__Internal")]
    private static extern void ApplyIceCandidate(string candidateData);
    
    [DllImport("__Internal")]
    private static extern void HelloString(string str);

    PhotonView View {
        get {return GetComponent<PhotonView>();}
    }

    void Awake() {
        Init();
    }

    public void InitWebRTC() {
        if (PhotonNetwork.IsMasterClient) {
            MakeOffer();
        }
    }

    public void Working(string thing) {
        HelloString(thing);
    }

    public void SendOffer(string sdp) {
        Debug.Log("Send offer");
        Debug.Log(sdp);
        View.RPC("HandleOffer", RpcTarget.OthersBuffered, sdp, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    [PunRPC]
    public void HandleOffer(string sdp, int callerId) {
        Debug.Log("Offer received");
        Debug.Log(sdp);
        MakeAnswer(sdp, callerId);
    }

    public void SendAnswer(string answerJson) {

        Dictionary<string, string> answer = JsonConvert.DeserializeObject<Dictionary<string, string>>(answerJson); 
        Debug.Log("Sending answer");

        string sdpString = answer["sdp"];
        int callerId = Int32.Parse(answer["callerId"]);

        Debug.Log("Parsed answerJson");
        Debug.Log(sdpString);
        Debug.Log(callerId);
        View.RPC("HandleAnswer", PhotonNetwork.LocalPlayer.Get(callerId), sdpString);
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
        ApplyIceCandidate(candidateData);
    }
}
