using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StealingLogic : MonoBehaviour {
	void OnCollisionEnter(Collision collision) {
		if(collision.gameObject.tag == "endpoint")
			GameManager.instance.OnItemInSafeZone(gameObject);
	}
}
