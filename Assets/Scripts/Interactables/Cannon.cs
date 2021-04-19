using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : Interactable {

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Ball") {
            this.transform.tag = "Filled";
        }
    }
}
