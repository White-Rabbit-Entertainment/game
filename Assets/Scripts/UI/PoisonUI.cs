using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoisonUI : MonoBehaviour {
  public GameObject poisonIndicator;
  public GameObject fullPoisonPrefab;
  public GameObject emptyPoisonPrefab;

  public void Update() {
  EmptyList();
    if (NetworkManager.instance.LocalPlayerPropertyIs<Team>("Team", Team.Traitor)) {
      if (((Traitor)NetworkManager.instance.GetMe()).hasPoison) {
        Instantiate(fullPoisonPrefab, poisonIndicator.transform);
      } else {
        Instantiate(emptyPoisonPrefab, poisonIndicator.transform);
      }
    }
  }

  public void EmptyList() {
    foreach (Transform child in poisonIndicator.transform) {
      Destroy(child.gameObject);
    }
  }
}
