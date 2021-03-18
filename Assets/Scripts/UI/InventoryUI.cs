using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour {

    public GameObject inventory;
    public GameObject inventoryItemPrefab;

    /// <summary> Adds an item to the player inventory in the UI. </summary>
    public void AddItem(Pocketable item) {
      // Instantiate a new inventory item
      GameObject uiItem = Instantiate(inventoryItemPrefab, inventory.transform);
      uiItem.GetComponentInChildren<Text>().text = item.name;
    }
}
