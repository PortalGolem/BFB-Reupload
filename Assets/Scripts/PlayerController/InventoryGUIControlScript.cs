
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryGUIControlScript : MonoBehaviour
{
    bool guiOpen = false;
    bool guiMoving = false;
    float guiProgress;
    float guiSize;
    [SerializeField] private float animationDuration;
    [SerializeField] private RectTransform rectTransform;
    playerControllerScript playerControllerScript;

    void Start()
    {
        playerControllerScript = FindFirstObjectByType<playerControllerScript>();
        guiSize = rectTransform.rect.width;
        guiProgress = -animationDuration/2;
    }
    public void toggleInventoryGUI(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (guiMoving == true)
            {
                return;
            }

            guiOpen = !guiOpen;

            if (guiOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                playerControllerScript.enabled = false;
            } else
            {
                Cursor.lockState = CursorLockMode.Locked;
                playerControllerScript.enabled = true;
            }

            guiMoving = true;
        }
    }

    public void Update()
    {
        if (guiMoving)
        {
            if (guiOpen && guiProgress + Time.deltaTime > 0)
            {
                guiProgress = 0;
                guiMoving = false;
            } else if (!guiOpen && guiProgress - Time.deltaTime < -animationDuration/2)
            {
                guiProgress = -animationDuration/2;
                guiMoving = false;
            } else
            {
                guiProgress += guiOpen ? Time.deltaTime : -Time.deltaTime;
            }

            rectTransform.position = new Vector3(guiProgress/(animationDuration/2) * guiSize, rectTransform.position.y, rectTransform.position.z);
        }
    }
}
