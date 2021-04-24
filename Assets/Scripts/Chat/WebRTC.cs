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
    private static extern void MakeOffer(int playerId);
    
    [DllImport("__Internal")]
    private static extern void MakeAnswer(string data);
    
    [DllImport("__Internal")]
    private static extern void ApplyAnswer(string data);
     
    [DllImport("__Internal")]
    private static extern void ApplyIceCandidate(string data);
    
    [DllImport("__Internal")]
    private static extern void HelloString(string str);

    PhotonView View {
        get {return GetComponent<PhotonView>();}
    }

    void Awake() {
        Init();
    }

    public void Call(int playerId) {
        MakeOffer(playerId);
    }

    public void SendOffer(string offerJson) {
        Dictionary<string, string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(offerJson); 
        // The player to send the offer to
        int receiverId = Int32.Parse(data["peerId"]);
        // Set the peerid to out id
        data["peerId"] = PhotonNetwork.LocalPlayer.ActorNumber;

        View.RPC("HandleOffer", PhotonNetwork.LocalPlayer.Get(receiverId), JsonConvert.SerializeObject(data));
    }

    [PunRPC]
    public void HandleOffer(string data) {
        MakeAnswer(data);
    }

    public void SendAnswer(string answerJson) {
        Dictionary<string, string> answer = JsonConvert.DeserializeObject<Dictionary<string, string>>(answerJson); 
        // Who is recieveing the answer
        int receiverId = Int32.Parse(answer["peerId"]);
        // Set the peer id back to our id (so the receiver knows who sent the answer)
        answer["peerId"] = PhotonNetwork.LocalPlayer.ActorNumber;
        View.RPC("HandleAnswer", PhotonNetwork.LocalPlayer.Get(receiverId), JsonConvert.SerializeObject(answer));
    }
    
    [PunRPC]
    public void HandleAnswer(string data) {
        ApplyAnswer(data);
    }
    
    public void SendIceCandidate(string data) {
        Dictionary<string, string> dataObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(data); 
        // Which player should we send the ice candidate to?
        int receiverId = Int32.Parse(dataObj["peerId"]);
        // Set the peerid to us (the player which send the ice candidate)
        dataObj["peerId"] = PhotonNetwork.LocalPlayer.ActorNumber;
        View.RPC("HandleIceCandidate", PhotonNetwork.LocalPlayer.Get(receiverId), JsonConvert.SerializeObject(dataObj));
    }

    [PunRPC]
    public void HandleIceCandidate(string candidateData) {
        ApplyIceCandidate(candidateData);
    }
}

