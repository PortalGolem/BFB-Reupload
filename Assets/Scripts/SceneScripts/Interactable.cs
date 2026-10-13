using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] UnityEvent interactionEvent;
    virtual public bool Interact()
    {
        interactionEvent.Invoke();

        return true;
    }
}
