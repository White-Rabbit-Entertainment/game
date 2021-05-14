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

    #if !UNITY_EDITOR && UNITY_WEBGL
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
        private static extern void RemovePeerConnection(int playerId);
        
        [DllImport("__Internal")]
        private static extern void SetPeerVolume(int playerId, float volume);
        
        [DllImport("__Internal")]
        private static extern float GetPeerVolume(int playerId);
        
        [DllImport("__Internal")]
        private static extern void HelloString(string str);
    #endif

    PhotonView View {
        get {return GetComponent<PhotonView>();}
    }

    public void Initialize() {
        #if !UNITY_EDITOR && UNITY_WEBGL
            Init();
        #endif
    }

    public void Call(int playerId) {
        #if !UNITY_EDITOR && UNITY_WEBGL
            MakeOffer(playerId);
        #endif
    }

    public void SendOffer(string offerJson) {
        Debug.Log("Got offer json");
        Dictionary<string, string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(offerJson); 
        Debug.Log("Got offer from json");
        // The player to send the offer to
        int receiverId = Int32.Parse(data["peerId"]);
        Debug.Log(receiverId);
        // Set the peerid to out id
        data["peerId"] = PhotonNetwork.LocalPlayer.ActorNumber.ToString();

        Debug.Log("Sending offer");
        View.RPC("HandleOffer", PhotonNetwork.LocalPlayer.Get(receiverId), JsonConvert.SerializeObject(data));
        Debug.Log("Sent offer");
    }

    [PunRPC]
    public void HandleOffer(string data) {
        #if !UNITY_EDITOR && UNITY_WEBGL
            MakeAnswer(data);
        #endif
    }

    public void SendAnswer(string answerJson) {
        Dictionary<string, string> answer = JsonConvert.DeserializeObject<Dictionary<string, string>>(answerJson); 
        // Who is recieveing the answer
        int receiverId = Int32.Parse(answer["peerId"]);
        // Set the peer id back to our id (so the receiver knows who sent the answer)
        answer["peerId"] = PhotonNetwork.LocalPlayer.ActorNumber.ToString();
        View.RPC("HandleAnswer", PhotonNetwork.LocalPlayer.Get(receiverId), JsonConvert.SerializeObject(answer));
    }
    
    [PunRPC]
    public void HandleAnswer(string data) {
        #if !UNITY_EDITOR && UNITY_WEBGL
            ApplyAnswer(data);
        #endif
    }
    
    public void SendIceCandidate(string data) {
        Dictionary<string, string> dataObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(data); 
        // Which player should we send the ice candidate to?
        int receiverId = Int32.Parse(dataObj["peerId"]);
        // Set the peerid to us (the player which send the ice candidate)
        dataObj["peerId"] = PhotonNetwork.LocalPlayer.ActorNumber.ToString();
        View.RPC("HandleIceCandidate", PhotonNetwork.LocalPlayer.Get(receiverId), JsonConvert.SerializeObject(dataObj));
    }

    [PunRPC]
    public void HandleIceCandidate(string candidateData) {
        #if !UNITY_EDITOR && UNITY_WEBGL
            ApplyIceCandidate(candidateData);
        #endif
    }

    public void EndCall(int playerId) {
        #if !UNITY_EDITOR && UNITY_WEBGL
            RemovePeerConnection(playerId);
        #endif
    }

    public void SetVolume(int playerId, float volume) {
        #if !UNITY_EDITOR && UNITY_WEBGL
            SetPeerVolume(playerId, volume);
        #endif
    }
    
    public float GetVolume(int playerId) {
        #if !UNITY_EDITOR && UNITY_WEBGL
            return GetPeerVolume(playerId);
        #endif
        return 0;
    }

    public void FailedToGet(bool gotAudio) {
        if (gotAudio) {
            Debug.Log("Failed to get access to camera.");
        } else {
            Debug.Log("Failed to get access to microphone and camera.");
        }
    }
}

