using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FoodRatingScript : MonoBehaviour
{
    [SerializeField] TMP_Text scoreDisplay;
    public int totalScore;
    [SerializeField] AnimationCurve AccuracyCurve;
    public List<MealData> foodToBeServed = new List<MealData>();

    public int evaluateScore(PlateScript plate)
    {
        int score = 0;
        List<FoodScript> foodScripts = plate.attachedFood;
        List<int> foodsChecked =  new List<int>();
        List<int> foodsCheckedGivenScore = new List<int>();
        for (int i = 0; i < foodScripts.Count; i++)
        {
            if (foodScripts[i].foodType == foodToBeServed[0].foods[i])
            {
                if (foodsChecked.Contains(i))
                {
                    foodsChecked.Remove(i);
                    score -= foodsCheckedGivenScore[i];
                    foodsCheckedGivenScore.Remove(i);
                }
                foodsChecked.Add(foodToBeServed[0].foods.IndexOf(foodScripts[i].foodType));
                float ingreedientScore = 0;
                int ingreedientsNotUsed = 0;
                for (int n = 0; n < foodScripts[i].effectIntensity.Length; n++)
                {
                    if (foodToBeServed[0].effects[n] == 0)
                    {
                        if (foodScripts[i].effectIntensity[n] != 0) 
                        {
                            ingreedientScore -= 1;
                        } else
                        {
                            ingreedientsNotUsed++;
                        }
                        continue;
                    }
                    ingreedientScore += AccuracyCurve.Evaluate(Mathf.Clamp(foodScripts[i].effectIntensity[n]/foodToBeServed[0].effects[n], 0f, 2f));
                }
                ingreedientScore /= foodScripts[i].effectIntensity.Length - ingreedientsNotUsed;
                ingreedientScore = Mathf.Max(0f, ingreedientScore);
                score += (int)Mathf.Floor(ingreedientScore * 100);
                foodsCheckedGivenScore.Add((int)Mathf.Floor(ingreedientScore * 50));
            } else if (foodToBeServed[0].foods.IndexOf(foodScripts[i].foodType) != -1)
            {
                int unservedFood = -1;
                for (int n = 0; n < foodToBeServed[0].foods.Count; n++)
                {
                    if (!foodsChecked.Contains(n) && foodToBeServed[0].foods[n] == foodScripts[i].foodType)
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
                        ingreedientScore += AccuracyCurve.Evaluate(Mathf.Clamp(foodScripts[i].effectIntensity[n]/foodToBeServed[0].effects[n], 0f, 2f));
                    }
                    ingreedientScore /= foodScripts[i].effectIntensity.Length;
                    
                    score += (int)Mathf.Floor(ingreedientScore * 50);
                }
                
            }
        }
        Debug.Log(score);
        score = 10 * (score/Mathf.Max(foodsChecked.Count, foodToBeServed[0].foods.Count));
        foodToBeServed.RemoveAt(0);
        Destroy(plate.gameObject);
        return score;
    }

    void OnTriggerEnter(Collider collision)
    {
        PlateScript collisionPlate = collision.gameObject.GetComponent<PlateScript>();
        
        if (!collisionPlate || collisionPlate.attachedFood.Count == 0) return;
        
        totalScore += evaluateScore(collisionPlate);

        scoreDisplay.text = totalScore.ToString();
    }
}

[Serializable]
public struct MealData
{
    public List<FoodType> foods;
    public EffectList effects;
}

[Serializable]
public class EffectList : ISerializationCallbackReceiver
{

    public float[] effects = new float[Enum.GetNames(typeof(FoodScript.Effect)).Length];
    [SerializeField] List<FoodScript.Effect> effectEnum = new List<FoodScript.Effect>();
    [SerializeField] List<float> effectValue = new List<float>();
    public void OnBeforeSerialize()
    {
        while (effectValue.Count < effectEnum.Count)
        {
            effectValue.Add(0f);
        }
        while (effectValue.Count > effectEnum.Count)
        {
            effectValue.RemoveAt(effectValue.Count - 1);
        }
    }

    public void OnAfterDeserialize()
    {
        effects = new float[Enum.GetNames(typeof(FoodScript.Effect)).Length];
        for (int i = 0; i < effectEnum.Count; i++)
        {
            effects[(int)effectEnum[i]] = effectValue[i];
        }
    }

    public float this[int key]
    {
        get { return effects[key]; }
        set { effects[key] = value; }
    }
}