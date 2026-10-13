using UnityEngine;

public class LeverScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    bool interacted = false;

    public void Interact()
    {
        interacted = true;
    }

    void Update()
    {
        if (interacted)
        {
            transform.position += (20 * Time.deltaTime) * Vector3.up;
        }
    }
}
