using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoisonUI : MonoBehaviour {
  public GameObject poisonIndicator;
  public GameObject fullPoisonPrefab;
  public GameObject emptyPoisonPrefab;

  public void Start() {
    if (NetworkManager.instance.LocalPlayerPropertyIs<Team>("Team", Team.Traitor)) {
      Instantiate(fullPoisonPrefab, poisonIndicator.transform);
    }
  }
  
  public void UsePoison() {
    poisonIndicator.DestroyChildren();
    Instantiate(emptyPoisonPrefab, poisonIndicator.transform);
  }
}
