using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemList", menuName = "Scriptable Objects/Item List")]
public class ItemList : ScriptableObject
{
    public List<ItemScript> items;
}