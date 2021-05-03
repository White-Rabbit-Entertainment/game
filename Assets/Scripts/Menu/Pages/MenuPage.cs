using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;

public class MenuPage: MonoBehaviourPunCallbacks {
  public MenuManager menuManager;

  public virtual void Open() {
    gameObject.SetActive(true);
    if (menuManager.currentPage != null) {
      menuManager.currentPage.Close();
    }
    menuManager.currentPage = this;
  }

  public virtual void Close() {
    gameObject.SetActive(false);
  }

}
