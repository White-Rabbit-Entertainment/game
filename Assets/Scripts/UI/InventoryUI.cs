using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour {

    public RawImage inventoryImage;

    /// <summary> Adds an item to the player inventory in the UI. </summary>
    public void AddItem(Pocketable item) {
      inventoryImage.gameObject.SetActive(true);
      inventoryImage.texture = item.image;
    }
    
    public void RemoveItem() {
      inventoryImage.gameObject.SetActive(false);
    }
}
