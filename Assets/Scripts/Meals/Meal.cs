using Photon.Pun;
using UnityEngine;

public class Meal : MonoBehaviour {
    public bool isPoisoned = false;
    public Color colour;
    
    [PunRPC]
    public void Poison() {
        isPoisoned = true;
    }
}