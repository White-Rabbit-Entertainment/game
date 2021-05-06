using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnityExample;

public class EmotionRecorder : MonoBehaviour
{
    public string lastEmotion;

    AsynchronousFaceDetectionWebCamTextureExample asynchronousFaceDetectionWebCamTextureExample;
    // Start is called before the first frame update
    void Start()
    {
        lastEmotion = "";
        asynchronousFaceDetectionWebCamTextureExample = GetComponent<AsynchronousFaceDetectionWebCamTextureExample>();

    }

    // Update is called once per frame
    void Update()
    {
        if (asynchronousFaceDetectionWebCamTextureExample.lastEmotion != lastEmotion) {
            lastEmotion = asynchronousFaceDetectionWebCamTextureExample.lastEmotion;
        }
    }
}
