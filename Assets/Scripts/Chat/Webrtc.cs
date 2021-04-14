using System;
using UnityEngine;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

public class Webrtc : MonoBehaviour {

    [DllImport("__Internal")]
    private static extern void Init();
    
    [DllImport("__Internal")]
    private static extern string MakeOffer(Action<string> action);
    
    [DllImport("__Internal")]
    private static extern void HelloString(string str);

    void Start() {
        Init();
        string sdp = MakeOffer(SendOffer);
        HelloString(sdp);
    }

    [MonoPInvokeCallback(typeof(Action<string>))]
    public static void SendOffer(string sdp)
    {
        HelloString(sdp);
    }

}

