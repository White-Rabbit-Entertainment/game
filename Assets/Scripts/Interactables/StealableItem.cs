using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StealableItem : Stealable {
    [PunRPC]
	void Steal() {
		Destroy(gameObject);
	}
}
