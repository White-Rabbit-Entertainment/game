using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{

    public GameObject inventory;
    public GameObject inventoryItemPrefab;

    /// <summary> Adds a task to the list of tasks in the UI. </summary>
    public void AddItem(Pocketable item) {
      // Instantiate a new task list item
      GameObject uiItem = Instantiate(inventoryItemPrefab, inventory.transform);
      uiItem.GetComponentInChildren<Text>().text = item.name;
    }
}
