using System;
using UnityEngine;

public class FoodScript : MonoBehaviour
{

    public enum Effect
    {
        Cooked,
        Waffle,
        Cut
    }

    [NonSerialized]
    public float[] effectIntensity = new float[Enum.GetNames(typeof(Effect)).Length];

    public FoodType foodType;

    public void effectChange(Effect effect, float changeAmount, bool set)
    {
        effectIntensity[(int)effect] = set ? changeAmount : effectIntensity[(int)effect] + changeAmount;

        switch (effect) {
            case Effect.Cooked:
                Cook(effectIntensity[(int)effect]);
                break;
            case Effect.Waffle:
                Waffle(effectIntensity[(int)effect]);
                break;
            case Effect.Cut:
                Cut(effectIntensity[(int)effect]);
                break;
            default:
                break;
        }
    }

    public virtual void Cook(float intensity)
    {
        Debug.Log(intensity);
    }

    public virtual void Waffle(float intensity)
    {
        
    }

    public virtual void Cut(float intensity)
    {
        
    }
}

public enum FoodType
{
    Potato,
    Meat,
    Bread,
    Cheese,
    Lettuce
}