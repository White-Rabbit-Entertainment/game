using Photon.Pun;
using UnityEngine;

public class Task : MonoBehaviour {
  public bool isCompleted = false;
  public string description;
    
  [PunRPC]
  public void Complete() {
    isCompleted = true;
  }
}
