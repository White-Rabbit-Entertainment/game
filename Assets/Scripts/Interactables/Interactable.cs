using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface Interactable {
  void PickUp(Transform pickUpDestination);
  void PutDown();
}
