using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

// Wrapper for WebRTC calling logic. If in unity editor all logic is disabled.
public class WebRTC : MonoBehaviour {

    // Import functions from jslib
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

    // Initalize WebRTC. This requests permission for microphone and webcam and
    // setups local stream.
    public void Initialize() {
        #if !UNITY_EDITOR && UNITY_WEBGL
            Init();
        #endif
    }

    // This create a peer connection and sends an offer to the player
    // with the provided player id.
    public void Call(int playerId) {
        #if !UNITY_EDITOR && UNITY_WEBGL
            MakeOffer(playerId);
        #endif
    }

    // Function used by Call to send the offer to the player. This uses an RPC
    // to call HandleOffer on the recieveing players machine.
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

    // Called by caller player. Given data (which contains the offer and the
    // player which made the call) it generates an asnwer and setups the peer
    // connection object. It then calls SendAnswer to send the answer back to
    // the caller player.
    [PunRPC]
    public void HandleOffer(string data) {
        #if !UNITY_EDITOR && UNITY_WEBGL
            MakeAnswer(data);
        #endif
    }

    // Uses RPC to send answer data back to player. This calls HandleAnswer on calling player machine.
    public void SendAnswer(string answerJson) {
        Dictionary<string, string> answer = JsonConvert.DeserializeObject<Dictionary<string, string>>(answerJson); 
        // Who is recieveing the answer
        int receiverId = Int32.Parse(answer["peerId"]);
        // Set the peer id back to our id (so the receiver knows who sent the answer)
        answer["peerId"] = PhotonNetwork.LocalPlayer.ActorNumber.ToString();
        View.RPC("HandleAnswer", PhotonNetwork.LocalPlayer.Get(receiverId), JsonConvert.SerializeObject(answer));
    }
   
    // Called by player that is called with the answer data. This then adds the
    // answer to peerconnection object.
    [PunRPC]
    public void HandleAnswer(string data) {
        #if !UNITY_EDITOR && UNITY_WEBGL
            ApplyAnswer(data);
        #endif
    }
   
    // Called when icecandidate received in the jslib from the ICE server.
    // Sends the ice candidate to the peer. This is done using an RPC which
    // calls HandleIceCandidate on the other player.
    public void SendIceCandidate(string data) {
        Dictionary<string, string> dataObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(data); 
        // Which player should we send the ice candidate to?
        int receiverId = Int32.Parse(dataObj["peerId"]);
        // Set the peerid to us (the player which send the ice candidate)
        dataObj["peerId"] = PhotonNetwork.LocalPlayer.ActorNumber.ToString();
        View.RPC("HandleIceCandidate", PhotonNetwork.LocalPlayer.Get(receiverId), JsonConvert.SerializeObject(dataObj));
    }

    // Add ice candidate to peer connection. This is called by the other player
    // with SendIceCandidate and includes the icecandidate data (and the player
    // id of who called).
    [PunRPC]
    public void HandleIceCandidate(string candidateData) {
        #if !UNITY_EDITOR && UNITY_WEBGL
            ApplyIceCandidate(candidateData);
        #endif
    }

    // Remove peer connection with player
    public void EndCall(int playerId) {
        #if !UNITY_EDITOR && UNITY_WEBGL
            RemovePeerConnection(playerId);
        #endif
    }

    // Sets the local volume of a peer connection (using the VideoPlayer
    // element)
    public void SetVolume(int playerId, float volume) {
        #if !UNITY_EDITOR && UNITY_WEBGL
            SetPeerVolume(playerId, volume);
        #endif
    }
   
    // Returns volume of video player element of a given peer connection.
    public float GetVolume(int playerId) {
        #if !UNITY_EDITOR && UNITY_WEBGL
            return GetPeerVolume(playerId);
        #endif
        return 0;
    }

    // Handles error when cannot get acess to webcam/camera.
    public void FailedToGet(bool gotAudio) {
        if (gotAudio) {
            Debug.Log("Failed to get access to camera.");
        } else {
            Debug.Log("Failed to get access to microphone and camera.");
        }
    }
}

