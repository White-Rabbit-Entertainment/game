using Photon.Pun;
using UnityEngine;

public class Meal : MonoBehaviour {
    public bool isPoisoned = false;
    public Color colour;

    public void Start() {
      DontDestroyOnLoad(gameObject);
    }
    
    [PunRPC]
    public void Poison() {
        isPoisoned = true;
    }

    [PunRPC]
    public void SetColour(float r, float g, float b) {
      colour = new Color(r,g,b);
    }
}
