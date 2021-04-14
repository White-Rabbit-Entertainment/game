using UnityEngine;
using System.Runtime.InteropServices;

public class Webrtc : MonoBehaviour {
    
    [DllImport("__Internal")]
    private static extern void Hello();

    void Start() {
        Hello();
    }
}

