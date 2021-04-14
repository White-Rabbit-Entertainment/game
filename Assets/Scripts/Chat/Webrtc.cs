using UnityEngine;
using System.Runtime.InteropServices;

public class Webrtc : MonoBehaviour {
    
    [DllImport("__Internal")]
    private static extern void SetupLocalStream();
    
    [DllImport("__Internal")]
    private static extern void Init();

    void Start() {
        Init();
    }
}

