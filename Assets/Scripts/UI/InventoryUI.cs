using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour {

    public RawImage inventory;

    /// <summary> Adds an item to the player inventory in the UI. </summary>
    public void AddItem(Pocketable item) {
      inventory.gameObject.SetActive(true);
      inventory.texture = item.image;
    }
}
