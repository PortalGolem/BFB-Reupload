using System;
using UnityEngine;
using UnityEngine.Events;

public class PickUpScript : Interactable
{
    [NonSerialized] public ItemScript item;
    public bool attachedToPlate = false;
    public bool onTopOfStack = false;
    override public bool Interact()
    {
        InventoryManager inventoryManager = FindFirstObjectByType<InventoryManager>();
        if (!attachedToPlate) {
            if (inventoryManager == null) Debug.Log("Failed To find inventory manager");
            if (item == null) Debug.Log("item set to null!");
            if (inventoryManager == null) {
                Debug.LogWarning("No inventory manager found!");
                return false;
            }
            inventoryManager.PickupItem(item, gameObject);

            return true;
        } else if (onTopOfStack)
        {
            Debug.Log("WOO code!");transform.parent.GetComponent<PlateScript>().RemoveTopItem();
            transform.parent = null;
            gameObject.AddComponent<Rigidbody>();
            attachedToPlate = false;
            onTopOfStack = false;
            inventoryManager.PickupItem(item, gameObject);
            return true;
        }
        return false;
    }
}
