using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRequiredInteractable : Interactable {
    public Pocketable requiredItem;

    public override bool CanInteract(Character character) {
        return base.CanInteract(character) && character.HasItem(requiredItem);
    }

    public override void AddTask() {
        GameObject pocketables = GameObject.Find("/Environment/Interactables/Pocketables");
        Pocketable[] possibleRequiredItems = pocketables.GetComponentsInChildren<Pocketable>();
        requiredItem = possibleRequiredItems[Random.Range(0,possibleRequiredItems.Length)];
        base.AddTask();
        requiredItem.IncludeInTask(task);

    }
}
