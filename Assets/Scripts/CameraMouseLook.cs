using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraMouseLook : MonoBehaviourPun
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    

    private float xRotation = 0f;
    

    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine) {
          Debug.Log("This is not mine Im out");
          Destroy(this);

          // Also destory the camera object
          Destroy(gameObject);
        } else {
          Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
	    float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        playerBody.Rotate(Vector3.up * mouseX);
	    xRotation -= mouseY;
	    xRotation = Mathf.Clamp(xRotation, -90f, 90f);
	    transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
