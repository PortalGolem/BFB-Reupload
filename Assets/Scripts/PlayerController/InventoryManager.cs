using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private static int INVENTORY_SIZE = 2;
    [SerializeField] private List<Image> inventoryDisplay;
    [SerializeField] Transform playerHeadTransform;
    [SerializeField] float dropItemMaxDistance;
    private ItemScript[] inventory = new ItemScript[INVENTORY_SIZE];
    private GameObject[] heldItems = new GameObject[INVENTORY_SIZE];
    private int selectedItem;

    public void SelectItem(int inventoryIndex)
    {
        if (inventory[selectedItem] != null)
        {
            heldItems[selectedItem].SetActive(false);
        }
        if (inventory[inventoryIndex] != null){
            
            heldItems[inventoryIndex].SetActive(true);
        }
        selectedItem = inventoryIndex;
    }

    public void DropItem(int inventoryIndex)
    {
        if (inventory[inventoryIndex] == null)
        {
            return;
        }
        inventoryDisplay[inventoryIndex].sprite = null;
        GameObject droppedItem = heldItems[inventoryIndex].GetComponent<HeldItemScript>().droppedItem;
        RaycastHit hit;
        if (Physics.Raycast(playerHeadTransform.position, playerHeadTransform.forward, out hit, dropItemMaxDistance)) 
        {
            droppedItem.transform.position = hit.point;
        } else if (Physics.Raycast(playerHeadTransform.position, Vector3.down, out hit, dropItemMaxDistance)) 
        {
            droppedItem.transform.position = hit.point;
        } else 
        {
            droppedItem.transform.position = playerHeadTransform.position;
        }

        droppedItem.SetActive(true);

        droppedItem.transform.rotation = Quaternion.AngleAxis(playerHeadTransform.rotation.eulerAngles.y, Vector3.up);

        droppedItem.transform.position = new Vector3(droppedItem.transform.position.x, droppedItem.transform.position.y - (droppedItem.GetComponent<Collider>().ClosestPointOnBounds(droppedItem.transform.position + Vector3.down * Mathf.Infinity) - droppedItem.transform.position).y, droppedItem.transform.position.z);

        Rigidbody rb = droppedItem.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }


        Destroy(heldItems[inventoryIndex]);
        inventory[inventoryIndex] = null;
    }

    public void DropItemInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            DropItem(selectedItem);
        }
    }

    public void SelctItemScroll(InputAction.CallbackContext context)
    {
        int scroll;

        if (context.performed){

            if (context.ReadValue<float>() > 0)
            {
                scroll = 1;    
            } else if (context.ReadValue<float>() < 0)
            {
                scroll = -1;
            } else
            {
                return;
            }

            if (selectedItem + scroll >= INVENTORY_SIZE)
            {
                SelectItem(0);
            } else if (selectedItem + scroll < 0)
            {
                SelectItem(INVENTORY_SIZE - 1);
            } else
            {
                SelectItem(selectedItem + scroll);
            }
        }
    }

    public void PickupItem(ItemScript item, GameObject droppedItem)
    {
        if (inventory[selectedItem] != null)
        {
            for (int i = 0; i < INVENTORY_SIZE; i++)
            {
                if (inventory[i] == null)
                {
                    heldItems[selectedItem].SetActive(false);
                    selectedItem = i;
                    InsertItemInInventory(i, item);
                    heldItems[i].GetComponent<HeldItemScript>().droppedItem = droppedItem;
                    droppedItem.SetActive(false);
                    return;
                }
            }
            //no empty slot avalible
            return;
        }
        else
        {
            InsertItemInInventory(selectedItem, item);
            heldItems[selectedItem].GetComponent<HeldItemScript>().droppedItem = droppedItem;
            droppedItem.SetActive(false);
            return;
        }
    }

    public void PickupItem(ItemScript item)
    {
        if (inventory[selectedItem] != null)
        {
            for (int i = 0; i < INVENTORY_SIZE; i++)
            {
                if (inventory[i] == null)
                {
                    InsertItemInInventory(i, item);
                    return;
                }
            }
            return;
        }
        else
        {
            InsertItemInInventory(selectedItem, item);
            return;
        }
    }

    private void InsertItemInInventory(int index, ItemScript item)
    {
        inventoryDisplay[index].sprite = ThumbnailGenerator.instance.textures[item.iconID];
        inventory[index] = item;
        heldItems[index] = item.GenerateHeldItem(playerHeadTransform);
    }
}
