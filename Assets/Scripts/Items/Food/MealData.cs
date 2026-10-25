using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MealData", menuName = "Scriptable Objects/MealData")]
public class MealData : ScriptableObject
{
    public string name = "";
    public List<FoodType> foods;
    public List<EffectList> effects;
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

    public int Count()
    {
        return effectEnum.Count;
    }
}