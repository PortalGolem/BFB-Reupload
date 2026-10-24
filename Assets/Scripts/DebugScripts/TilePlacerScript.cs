#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class TilePlacerScript : MonoBehaviour
{
    [SerializeField] private List<Transform> connections = new List<Transform>();

    [MenuItem("Edit/Re-place Tiles")]
    private static void HelloWorld()
    {
        TilePlacerScript[] tilePlacers = FindObjectsByType<TilePlacerScript>(FindObjectsSortMode.None);
        print("Hello!" + tilePlacers.Length);
        foreach (TilePlacerScript tile in tilePlacers)
        {
            print("Tried to place tile");
            //Assume the first child is the actual model
            int currentConnection = 0;
            for (int i = 1; i < tile.transform.childCount; i++)
            {
                if (currentConnection == tile.connections.Count)
                {
                    break;
                }
                Transform nextTile = tile.transform.GetChild(i);
                if (tile.connections.Contains(nextTile))
                {
                    continue;
                }
                nextTile.position = tile.connections[currentConnection].position;
                nextTile.rotation = tile.connections[currentConnection].rotation;
                currentConnection++;
            }
        }
    }
}

#endif