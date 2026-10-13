using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionScript : MonoBehaviour
{
    [SerializeField] Transform headTransform;
    [SerializeField] float interactionDistance;
    [SerializeField] LayerMask interactableMask;
    [SerializeField] GameObject interactableMessage;
    private bool hasInteracted = false;

    void Update()
    {
        bool foundInteractable = false;
        RaycastHit hit;
        if (Physics.Raycast(headTransform.position, headTransform.forward, out hit, interactionDistance, interactableMask, QueryTriggerInteraction.Ignore)){


            Interactable interactable = hit.collider.gameObject.GetComponent<Interactable>();
            if (interactable != null)
            {
                foundInteractable = true;
                interactableMessage.SetActive(true);
                if (hasInteracted)
                {
                    interactable.Interact();
                }
            }
        }
        
        if (!foundInteractable)
        {
            interactableMessage.SetActive(false);
        }

        hasInteracted = false;
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            hasInteracted = true;
        }
    }
}
