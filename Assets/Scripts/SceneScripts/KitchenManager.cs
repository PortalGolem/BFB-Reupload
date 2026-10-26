
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class KitchenManager : MonoBehaviour
{

    //Get everything ready

    [SerializeField] List<GameObject> kitchenBoundries = new();
    [SerializeField] List<DoorScript> kitchenDoors = new();
    [SerializeField] GameObject orderCounter;
    [SerializeField] FoodRatingScript foodRatingScript;
    [SerializeField] ReceiptPrinterScript receiptPrinter;
    [SerializeField] TMP_Text textDisplayMiliseconds;
    [SerializeField] TMP_Text textDisplaySeconds;
    [SerializeField] TMP_Text textDisplayMinutes;
    [SerializeField] EventSequence testEventSequence;

    //This is all the day specific information. This could be converted into a scriptable object
    //and swapped out if we want multiple cooking sessions in a single day
    [SerializeField] List<MealData> possibleMealsToday;
    [SerializeField] MealData firstMealToday;
    [SerializeField] int quotaAmmount;
    [SerializeField] int possibleStrikes;
    [SerializeField] int numberOfContiguousOrders;
    [SerializeField] int scoreThreshold;
    [SerializeField] TMP_Text winText;

    private float kitchenTime;
    private EventSequence eventSequence;
    private int SequencePriority;
    private bool SequenceOverride;



    private float kitchenTimer;
    private bool isKitchenTimerTimeUpCalled = true;
    private int strikesGiven;
    private int currentOrder;
    private int quota;
    private bool youWin = false;
    private List<MealData> chosenMeals = new List<MealData>();

    void Start()
    {
    }

    void Update()
    {
        if (kitchenTimer > 0)
        {
            kitchenTimer -= Time.deltaTime;
            textDisplayMinutes.text = ((int)math.floor(kitchenTimer / 60)).ToString();
            textDisplaySeconds.text = ((int)math.floor(kitchenTimer % 60)).ToString();
            textDisplayMiliseconds.text = ((int)math.floor(kitchenTimer * 100 % 100)).ToString();
        }
        else if (!isKitchenTimerTimeUpCalled)
        {
            timeIsUp();
            isKitchenTimerTimeUpCalled = true;
        }
    }

    public bool startCookingTime(float timeToCook)
    {
        return startCookingTime(timeToCook, testEventSequence);
    }

    public bool startCookingTime(float timeToCook, EventSequence cookingStartEvents, int startEventPriority = 0, bool startEventOverride = false)
    {
        bool startedCooking = true;
        // make sure now is a good time

        kitchenTime = timeToCook;
        eventSequence = cookingStartEvents;
        SequencePriority = startEventPriority;
        SequenceOverride = startEventOverride;

        //Set up everything
        startedCooking = startedCooking && SetupKitchen();
        startedCooking = startedCooking && DisableShip();

        print(startedCooking);

        return startedCooking;
    }

    bool SetupKitchen()
    {
        bool success = true;

        // enable invisible walls
        foreach (GameObject boundry in kitchenBoundries)
        {
            boundry.SetActive(true);
        }
        // close necesary doors
        foreach (DoorScript door in kitchenDoors)
        {
            door.Close();
        }

        orderCounter.SetActive(true);

        // play monster setup animation
        // music transition

        //generate the list of meals
        if (firstMealToday == null)
        {
            generateMealData();
        }
        else
        {
            generateMealData(true);
        }
        for (int i = 0; i < possibleStrikes + quota - 1; i++)
        {
            generateMealData();
        }

        // start timer
        isKitchenTimerTimeUpCalled = false;
        kitchenTimer = kitchenTime;

        // run kitchen event
        if (SequenceOverride)
        {
            DialogueManagerScript.instance.AddEventToQueue(eventSequence, true, true);
        }
        else
        {
            DialogueManagerScript.instance.AddEventToQueue(eventSequence, SequencePriority);
        }

        // print first orders
        for (int i = 0; i < numberOfContiguousOrders; i++)
        {
            foodRatingScript.foodToBeServed.Add(chosenMeals[currentOrder]);
            receiptPrinter.printReceipt(generateOrderReceipt());
        }
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

    public void ProcessBurger(int score)
    {
        // print new order
        if (currentOrder < chosenMeals.Count)
        {
            foodRatingScript.foodToBeServed.Add(chosenMeals[currentOrder]);
            receiptPrinter.printReceipt(generateOrderReceipt());
        }

        //checkResult
        if (score > scoreThreshold)
        {
            //stuff that runs when quota is added
            quota++;

            if (quota >= quotaAmmount)
            {
                endKitchen();
            }
        }
        else
        {
            //stuff that happens when player makes a mistake
            strikesGiven++;
            //monster anger animation

            if (strikesGiven > possibleStrikes)
            {
                killPlayer();
            }
        }
    }

    // After the cook cycle

    public void timeIsUp()
    {
        if (!youWin)
        {
            // display score
            kitchenTimer = 0;
            // kill player
            killPlayer();
        }
    }

    void killPlayer()
    {
        Debug.Log("TryKillPlayer");
        FindFirstObjectByType<DeathScript>().killPlayer();
    }

    void endKitchen()
    {
        // play any win effects (this could go to the event player)
        //freeze the clock
        kitchenTimer = 0;
        print("You Win!");
        winText.text = "You Win!";
        youWin = true;
        // music transition
        // monster leave animation

        //reopen the kitchen so the player can leave
        foreach (DoorScript door in kitchenDoors)
        {
            door.Open();
        }

        foreach (GameObject boundry in kitchenBoundries)
        {
            boundry.SetActive(false);
        }

        EnableShip();
        // start any other events here
    }

    string generateOrderReceipt()
    {
        string text = "";

        //Header
        text += "Monster Burger\n";
        text += chosenMeals[currentOrder].name + "\n";

        for (int i = 0; i < chosenMeals[currentOrder].foods.Count; i++)
        {
            text += " " + chosenMeals[currentOrder].foods[i].ToString() + "\n";
            for (int n = 0; n < chosenMeals[currentOrder].effects[i].Count() + 1; n++)
            {
                float intensity = chosenMeals[currentOrder].effects[i][n];
                if (intensity > 0)
                {
                    if (intensity == 1)
                    {
                        text += "  " + ((FoodScript.Effect)n).ToString() + "\n";
                    }
                    else
                    {
                        text += "  " + ((FoodScript.Effect)n).ToString() + " " + intensity + "\n";
                    }
                }
            }
        }

        currentOrder++;
        return text;
    }

    MealData generateMealData(bool isFirstMealToday = false)
    {
        if (isFirstMealToday)
        {
            chosenMeals.Add(firstMealToday);
            return firstMealToday;
        }
        int nextMeal = UnityEngine.Random.Range(0, possibleMealsToday.Count);

        chosenMeals.Add(possibleMealsToday[nextMeal]);

        return possibleMealsToday[nextMeal];
    }
}
