using UnityEngine;

public class KitchenStartScript : MonoBehaviour
{
    bool hasStarted = false;
    [SerializeField] float kitchenTime;
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<playerControllerScript>() && !hasStarted)
        {
            hasStarted = true;
            FindFirstObjectByType<KitchenManager>().startCookingTime(kitchenTime);
            //Hello
        }
    }
}
