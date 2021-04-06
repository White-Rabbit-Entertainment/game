using UnityEngine;

public class FloorDetector {
  public void OnTriggerEnter(Collider collider) {
    Vector3 position = collider.transform.position;
    collider.transform.position = new Vector3(position.x, 1, position.z);
  }
}
