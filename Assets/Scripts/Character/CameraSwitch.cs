
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public GameObject camera0;
    public GameObject camera1;
void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            camera0.SetActive(true);
            camera1.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            camera0.SetActive(false);
            camera1.SetActive(true);
        }
    }


}