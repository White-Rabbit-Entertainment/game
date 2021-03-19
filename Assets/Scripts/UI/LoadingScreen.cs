using UnityEngine;

public class LoadingScreen : MonoBehaviour {
  
  public GameObject loadingScreen;
  public PlayersUI playersUI; 

  void Update() {
    if (GameManager.instance.SceneLoaded()) {
      playersUI.Init();
      Destroy(loadingScreen);
      Destroy(this);
    }
  }
}
