
using UnityEngine;

public class KitchenManager : MonoBehaviour
{

    //Get everything ready

    public bool startCookingTime()
    {
        bool startedCooking = true;
        // make sure now is a good time

        //Set up everything
        startedCooking = startedCooking && SetupKitchen();
        startedCooking = startedCooking && DisableShip();

        return startedCooking;
    }

    bool SetupKitchen()
    {
        bool success = true;
        // add invisible walls/clsoe door
        // play monster setup animation
        // music transition
        // count down animation
        // generate orders for double the current quota. (This might be better to move to a different thread if it causes lag.)
        // start timer
        // run kitchen event
        // enable order counter
        // print first order
        return success;
    }

    bool DisableShip()
    {
        bool success = true;
        // disable the normal monster if it still exists
        return success;
    }

    bool EnableShip()
    {
        bool success = true;
        // undo all the disable ship stuff
        return success;
    }

    // During the cook cycle

    public void ProcessBurger()
    {
        // print new order
        // run test algorithm on existing orders for submission
        // take best score.
        // if its above the threshold play success effects
        // add to quota
        // otherwise add a strike

        // if strikes are at limit kill player

        // if quota is met
        endKitchen();
    }

    void Update()
    {
        //update timer 
    }

    // After the cook cycle

    public void timeIsUp()
    {
        // display score
        // kill player if they did not meet quota.
        endKitchen();
    }

    void endKitchen()
    {
        // play any win effects (this could go to the event player)
        // music transition
        // monster leave animation
        // kitchen door unlocks.
        EnableShip();
        // start any other events here
    }


}
