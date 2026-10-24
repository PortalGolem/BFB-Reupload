using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public void Close()
    {
        print("Ahh I've been closed");
        gameObject.SetActive(true);
    }

    public void Open()
    {
        print("Ahh I've been opened");
        gameObject.SetActive(false);
    }

    public void Toggle()
    {
        if (gameObject.activeInHierarchy)
        {
            Open();
        }
        else
        {
            Close();
        }
    }
}
