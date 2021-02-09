using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restartwithmovetargettoendpoint : MonoBehaviour
{
	void OnCollisionEnter(Collision collision){
		if(collision.gameObject.tag == "endpoint")
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
