using System;
using UnityEngine;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

public class WebRTC : MonoBehaviour {

    [DllImport("__Internal")]
    private static extern void Init();
    
    [DllImport("__Internal")]
    private static extern string MakeOffer();
    
    [DllImport("__Internal")]
    private static extern void HelloString(string str);

    void Start() {
        Init();
        string sdp = MakeOffer();
        HelloString(sdp);
    }

    public void Working(string thing) {
        HelloString(thing);
    }
}

