using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FoodRatingScript : MonoBehaviour
{
    [SerializeField] TMP_Text scoreDisplay;
    public int totalScore;
    [SerializeField] AnimationCurve AccuracyCurve;
    [SerializeField] KitchenManager kitchenManager;
    [DoNotSerialize] public List<MealData> foodToBeServed = new List<MealData>();

    public int evaluateScore(PlateScript plate)
    {
        int finalScore = 0;
        int usedFoodScriptIndex = 0;
        for (int currentTestFood = 0; currentTestFood < foodToBeServed.Count; currentTestFood++)
        {
            int score = 0;
            List<FoodScript> foodScripts = plate.attachedFood;
            List<int> foodsChecked = new List<int>();
            List<int> foodsCheckedGivenScore = new List<int>();
            for (int i = 0; i < foodScripts.Count; i++)
            {
                if (foodToBeServed.Count <= 0)
                {
                    return 0;
                }
                if (foodScripts[i].foodType == foodToBeServed[currentTestFood].foods[i])
                {
                    if (foodsChecked.Contains(i))
                    {
                        foodsChecked.Remove(i);
                        score -= foodsCheckedGivenScore[i];
                        foodsCheckedGivenScore.Remove(i);
                    }
                    foodsChecked.Add(foodToBeServed[currentTestFood].foods.IndexOf(foodScripts[i].foodType));
                    float ingreedientScore = 0;
                    int ingreedientsNotUsed = 0;
                    for (int n = 0; n < foodScripts[i].effectIntensity.Length; n++)
                    {
                        if (foodToBeServed[currentTestFood].effects[i][n] == 0)
                        {
                            if (foodScripts[i].effectIntensity[n] != 0)
                            {
                                ingreedientScore -= 1;
                            }
                            else
                            {
                                ingreedientsNotUsed++;
                            }
                            continue;
                        }
                        ingreedientScore += AccuracyCurve.Evaluate(Mathf.Clamp(foodScripts[i].effectIntensity[n] / foodToBeServed[currentTestFood].effects[i][n], 0f, 2f));
                    }
                    ingreedientScore /= foodScripts[i].effectIntensity.Length - ingreedientsNotUsed;
                    ingreedientScore = Mathf.Max(0f, ingreedientScore);
                    score += (int)Mathf.Floor(ingreedientScore * 100);
                    foodsCheckedGivenScore.Add((int)Mathf.Floor(ingreedientScore * 50));
                }
                else if (foodToBeServed[currentTestFood].foods.IndexOf(foodScripts[i].foodType) != -1)
                {
                    int unservedFood = -1;
                    for (int n = 0; n < foodToBeServed[currentTestFood].foods.Count; n++)
                    {
                        if (!foodsChecked.Contains(n) && foodToBeServed[currentTestFood].foods[n] == foodScripts[i].foodType)
                        {
                            unservedFood = n;
                            break;
                        }
                    }

                    if (unservedFood != -1)
                    {
                        foodsChecked.Add(unservedFood);
                        float ingreedientScore = 0;
                        for (int n = 0; n < foodScripts[i].effectIntensity.Length; n++)
                        {
                            //A problem with this score calculation is that you have to be more acurate for smaller intesities of the effect resulting in a reverse difficulty curve as the game progresses.
                            //We could either change the curves to account for this or change this calculation method
                            ingreedientScore += AccuracyCurve.Evaluate(Mathf.Clamp(foodScripts[i].effectIntensity[n] / foodToBeServed[currentTestFood].effects[i][n], 0f, 2f));
                        }
                        ingreedientScore /= foodScripts[i].effectIntensity.Length;

                        score += (int)Mathf.Floor(ingreedientScore * 50);
                    }

                }
            }
            score = 10 * (score / Mathf.Max(foodsChecked.Count, foodToBeServed[currentTestFood].foods.Count));
            if (score > finalScore)
            {
                finalScore = score;
                usedFoodScriptIndex = currentTestFood;
            }
        }
        foodToBeServed.RemoveAt(usedFoodScriptIndex);
        Destroy(plate.gameObject);
        return finalScore;
    }

    void OnTriggerEnter(Collider collision)
    {
        PlateScript collisionPlate = collision.gameObject.GetComponent<PlateScript>();

        if (!collisionPlate || collisionPlate.attachedFood.Count == 0) return;

        int score = evaluateScore(collisionPlate);

        kitchenManager.ProcessBurger(score);
        totalScore += score;

        scoreDisplay.text = totalScore.ToString();
    }
}