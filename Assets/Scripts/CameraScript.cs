using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraScript : MonoBehaviour
{
    static WebCamTexture myWebcamTexture; 
    void Start()
    {
        if (myWebcamTexture == null)
            myWebcamTexture = new WebCamTexture();
        myWebcamTexture.requestedFPS = 60;
        GetComponent<RawImage>().texture = myWebcamTexture;

        if (!myWebcamTexture.isPlaying)
            myWebcamTexture.Play();

    }
}


// using Microsoft.CSharp.RuntimeBinder.Binder.Convert;

// public class CameraScript : MonoBehaviour {
// 	void Start () {
//         // var options = new Dictionary<string, object>();
//         // options["Arguments"] = new[] {"--onnx", "--opt", "3d"};
//         var engine = Python.CreateEngine();

//         ICollection<string> searchPaths = engine.GetSearchPaths();

//         //Path to the folder of greeter.py
//         searchPaths.Add("../../year3/Game/3DDFA_V2");
//         //Path to the Python standard library
//         searchPaths.Add("../Plugins/Lib/");
//         Debug.Log(searchPaths);
//         engine.SetSearchPaths(searchPaths);
        
//         // List<string> argv = new List<string>();
//         // argv.Add("--onnx --opt 3d");
//         // engine.GetSysModule().SetVariable("argv", argv);

//         engine.ExecuteFile("../../year3/Game/3DDFA_V2/test_unity.py");
//         // GetComponent<RawImage>().texture = py;
        
//     }
// }