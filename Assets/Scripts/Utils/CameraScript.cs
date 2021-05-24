using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// Maps webcam feed to unity texture
public class CameraScript : MonoBehaviour {
    static WebCamTexture myWebcamTexture; 

    void Start() {
        if (myWebcamTexture == null) myWebcamTexture = new WebCamTexture();
        myWebcamTexture.requestedFPS = 60;
        GetComponent<RawImage>().texture = myWebcamTexture;

        if (!myWebcamTexture.isPlaying) myWebcamTexture.Play();

    }
}
