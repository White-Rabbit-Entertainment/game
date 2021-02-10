using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StealingLogic : MonoBehaviour {
	GameManager manager;

	void Start() {
        manager = new GameManager();
    }

	void OnCollisionEnter(Collision collision){
		if(collision.gameObject.tag == "endpoint")
			manager.OnItemInSafeZone(gameObject);
	}
}
