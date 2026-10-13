using System.Collections.Generic;
using UnityEngine;

public class PlateScript : MonoBehaviour
{
    public List<FoodScript> attachedFood = new List<FoodScript>();

    void OnCollisionEnter(Collision collision)
    {
        FoodScript otherFoodScript = collision.gameObject.GetComponent<FoodScript>();
        if (otherFoodScript != null && !attachedFood.Contains(otherFoodScript))
        {
            if (attachedFood.Count > 0) attachedFood[attachedFood.Count - 1].gameObject.GetComponent<PickUpScript>().onTopOfStack = false;
            attachedFood.Add(otherFoodScript);
            Destroy(collision.rigidbody);
            PickUpScript pickUpScript = collision.gameObject.GetComponent<PickUpScript>();
            pickUpScript.attachedToPlate = true;
            pickUpScript.onTopOfStack = true;
            collision.transform.SetParent(transform, true);
        }
    }

    public void RemoveTopItem()
    {
        attachedFood.RemoveAt(attachedFood.Count - 1);
        if (attachedFood.Count > 0) attachedFood[attachedFood.Count - 1].gameObject.GetComponent<PickUpScript>().onTopOfStack = true;
    }
}
