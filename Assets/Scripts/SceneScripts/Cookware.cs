using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Cookware : MonoBehaviour
{
    [SerializeField] private FoodScript.Effect effect;
    [SerializeField] private bool singleUse;
    void OnTriggerStay(Collider other)
    {
        FoodScript otherFood = other.GetComponent<FoodScript>();
        if (otherFood)
        {
            otherFood.effectChange(effect, singleUse ? 1f : Time.deltaTime, singleUse);
        }
    }
}
