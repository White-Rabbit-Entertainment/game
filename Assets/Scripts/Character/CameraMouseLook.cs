using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;


public class CameraMouseLook : MonoBehaviourPun {
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    
    private float xRotation = 0f;
    
    void Start() {
        //Destroy this script and the camera object if the player does not own this camera
        if (photonView != null && !photonView.IsMine) {
          Destroy(this);
          Destroy(gameObject);
        }
    }

    void Update() {
      //Get mouse x and y inputs based on mouse sensitivity
      float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
	    float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

      //use mouse inputs to rotate the player body and the camera
      playerBody.Rotate(Vector3.up * mouseX);
	    xRotation -= mouseY;
	    xRotation = Mathf.Clamp(xRotation, -90f, 90f);
	    transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
