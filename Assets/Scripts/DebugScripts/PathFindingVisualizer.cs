using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class PathFindingVisualizer : MonoBehaviour
{
    public Vector3 targetAproximation;
    public List<hallVertexScript> points = new List<hallVertexScript>();

    //List<int> path;
    //bool pathGenerated = false;
    // void Start()
    // {
    //     path = hallTraverserScript.GenerateAstar();
    //     pathGenerated = true;

    // }

    // void Update()
    // {
    //     dpath = hallTraverserScript.GenerateAstar();
    // }
    #if UNITY_EDITOR

    void OnDrawGizmos()
    {
        if (Application.isEditor)
        {
            for (int wayPointIndex = 0; wayPointIndex < transform.childCount; wayPointIndex++)
            {
                Transform point = transform.GetChild(wayPointIndex);
                hallVertexScript vertexScript = point.GetComponent<hallVertexScript>();
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(point.position, 0.2f);

                foreach (int connection in vertexScript.connections)
                {
                    Gizmos.DrawLine(point.position, transform.GetChild(connection).position);
                }
            }
        }
        // if (pathGenerated){
        // Gizmos.color = Color.green;
        //     for (int i = 0; i < path.Count - 1; i++)
        //     {
        //         Transform point = transform.GetChild(path[i]);
        //         Gizmos.DrawSphere(point.position, 0.2f);
        //         Gizmos.DrawLine(point.position, transform.GetChild(path[i+1]).position);
        //     }
        //     Gizmos.DrawSphere(transform.GetChild(path[^1]).position, 0.2f);
        // }

        Gizmos.color = Color.purple;
        Gizmos.DrawSphere(targetAproximation, .5f);
    }
    #endif
}
#if UNITY_EDITOR

[CustomEditor(typeof(PathFindingVisualizer))]
public class RenderGUI : Editor
{
    private hallVertexScript lastVertexConnectionTouched;
    int lastVertexTouchedIndex;
    void OnSceneGUI()
    {
        Transform transform = FindFirstObjectByType<PathFindingVisualizer>().transform;
        PathFindingVisualizer pathFindingVisualizer = transform.gameObject.GetComponent<PathFindingVisualizer>();
        Undo.RecordObject(pathFindingVisualizer, "Pathfinding Path Changes");
        PrefabUtility.RecordPrefabInstancePropertyModifications(pathFindingVisualizer);
        for (int wayPointIndex = 0; wayPointIndex < transform.childCount; wayPointIndex++)
        {
            Transform point = transform.GetChild(wayPointIndex);
            point.position = Handles.FreeMoveHandle(point.position, .3f, Vector3.zero, Handles.CircleHandleCap);
            hallVertexScript vertexScript = point.GetComponent<hallVertexScript>();
            
            Undo.RecordObject(vertexScript, "Pathfinding Path Changes");
            PrefabUtility.RecordPrefabInstancePropertyModifications(vertexScript);

            //add vertex tool
            Handles.color = Color.green;
            if (Handles.Button(point.position + (Vector3.left * .4f), Quaternion.identity, .3f, .15f, Handles.CubeHandleCap))
            {
                //create a new point with the specified information and position based off of the position of the handle
                vertexScript.connections.Add(transform.childCount);
                GameObject newPoint = new GameObject("Vertex (" + transform.childCount + ")");
                newPoint.transform.SetParent(transform);
                newPoint.transform.position = point.position + (Vector3.left * .4f);
                newPoint.AddComponent<hallVertexScript>();
                hallVertexScript vertex = newPoint.GetComponent<hallVertexScript>();
                vertex.connections.Add(wayPointIndex);
                pathFindingVisualizer.points.Add(vertex);

            }
            //connect existing vertexes by pressing a button on each vertex to be connected
            Handles.color = Color.lightSteelBlue;
            if (Handles.Button(point.position + (Vector3.right * .4f), Quaternion.identity, .3f, .15f, Handles.ConeHandleCap))
            {
                if (lastVertexConnectionTouched == null)
                {
                    lastVertexConnectionTouched = vertexScript;
                    lastVertexTouchedIndex = wayPointIndex;
                }
                else
                {
                    if (lastVertexTouchedIndex != wayPointIndex && !vertexScript.connections.Contains(lastVertexTouchedIndex)){
                        vertexScript.connections.Add(lastVertexTouchedIndex);
                        lastVertexConnectionTouched.connections.Add(wayPointIndex);
                    }
                    lastVertexConnectionTouched = null;
                }
            }
            //Delete vertex button
            Handles.color = Color.crimson;
            if (Handles.Button(point.position + (Vector3.forward * .4f), Quaternion.identity, .3f, .15f, Handles.SphereHandleCap))
            {
                foreach (int connection in vertexScript.connections)
                {
                    transform.GetChild(connection).GetComponent<hallVertexScript>().connections.Remove(wayPointIndex);
                }
                pathFindingVisualizer.points.Remove(vertexScript);

                for (int vertexToRepair = 0; vertexToRepair < transform.childCount; vertexToRepair++)
                {
                    hallVertexScript vertexToRepairScript = transform.GetChild(vertexToRepair).GetComponent<hallVertexScript>();
                    Undo.RecordObject(vertexToRepairScript.gameObject, "Remove Vertex");
                    for (int connection = 0; connection < vertexToRepairScript.connections.Count; connection++)
                    {
                        if (vertexToRepairScript.connections[connection] > wayPointIndex)
                        {
                            vertexToRepairScript.connections[connection]--;
                        }
                    }
                }
                DestroyImmediate(point.gameObject);
                break;
            }
            //Add buttons for all connections if they havn't been added yet. hello
            foreach (int connection in vertexScript.connections)
            {
                //Only loop through connections which have not been created
                if (connection > wayPointIndex)
                {
                    Transform otherPoint = transform.GetChild(connection);
                    float distance = Vector3.Distance(point.position, otherPoint.position);
                    float offset = .25f;
                    
                    if (distance < 1)
                    {
                        offset = distance/4;
                    }

                    //Add Vertex Button
                    Handles.color = Color.green;
                    if (Handles.Button((otherPoint.position - point.position).normalized * ((distance * .5f) - offset) + point.position, quaternion.AxisAngle(point.position - otherPoint.position, 0f), .3f, .5f, Handles.CubeHandleCap)) {
                        hallVertexScript otherVertexScript =  otherPoint.GetComponent<hallVertexScript>();
                        vertexScript.connections.Remove(connection);
                        otherVertexScript.connections.Remove(wayPointIndex);
                        //Add the new point
                        vertexScript.connections.Add(transform.childCount);
                        otherVertexScript.connections.Add(transform.childCount);
                        GameObject newPoint = new GameObject("Vertex");
                        newPoint.transform.SetParent(transform);
                        newPoint.transform.position = (otherPoint.position - point.position).normalized * ((distance * .5f) - offset) + point.position;
                        newPoint.AddComponent<hallVertexScript>();
                        hallVertexScript vertex = newPoint.GetComponent<hallVertexScript>();
                        vertex.connections.Add(wayPointIndex);
                        vertex.connections.Add(connection);
                        pathFindingVisualizer.points.Add(vertex);
                        break;
                    }

                    //Delete connection handle
                    Handles.color = Color.maroon;
                    if (Handles.Button((otherPoint.position - point.position).normalized * ((distance * .5f) + offset) + point.position, quaternion.AxisAngle(point.position - otherPoint.position, 0f), .3f, .5f, Handles.SphereHandleCap))
                    {
                        vertexScript.connections.Remove(connection);
                        otherPoint.GetComponent<hallVertexScript>().connections.Remove(wayPointIndex);
                        break;
                    }
                }
            }
        }
    }
}
#endif