using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    
}
