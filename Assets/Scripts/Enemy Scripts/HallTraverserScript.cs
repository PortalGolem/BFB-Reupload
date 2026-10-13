using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;
using System;
public class HallTraverserScript : MonoBehaviour
{
    [SerializeField] public PathFindingVisualizer pathScript;

    private int targetVert1 = 0;
    private int targetVert2 = 0;

    public Vector3 targetAproximation = new Vector3();

    public List<int> GenerateAstar(Vector3 target, int startingPoint)
    {
        setTarget(target);
        List<int> path = A_StarFindPath(pathScript.points, startingPoint, new List<int> {targetVert1, targetVert2});
        return path;
    }

    //Calculates the shortest path between a start and one or more goals. Only calculates the shortest path to one goal.
    private List<int> A_StarFindPath(List<hallVertexScript> verticies, int start, List<int> goals)
    {
        //initialize the heap and lists for all the data that needs to be stored for each point.
        PriorityQueue heap = new PriorityQueue();
        heap.Enqueue(calculateHeuristic(verticies, start, goals), 0);
        int currentIndex = 1;
        List<int> allpoints = new List<int> {start};
        List<int> lastPoint = new List<int> {-1};
        List<float> pointTravelDistance = new List<float> {0};
        List<float> totalCost = new List<float> {heap.Peek().cost};

        //check each point in the priority queue one by one
        for (; ; )
        {
            //exit the loop if the queue is empty
            if (heap.queue.Count == 0)
            {
                break;
            }

            //get the next element
            int testIndex = heap.Dequeue().index;
            hallVertexScript testPoint = verticies[allpoints[testIndex]];

            //return the path to the goal if a goal is found
            if (goals.Contains(allpoints[testIndex]))
            {
                //for (int i = 1; i < allpoints.Count; i++)wa
                //{
                //    Debug.Log(allpoints[i].ToString() + " connects to " + allpoints[lastPoint[i]].ToString() + " The total cost to this point was " + totalCost[i].ToString());
                //}
                //Debug.Log(testIndex);
                List<int> path = new List<int> ();
                int lastIndex = testIndex;
                int ticker = 0;
                while (lastIndex != -1 && ticker < 100)
                {
                    ticker++;
                    path.Add(allpoints[lastIndex]);
                    lastIndex = lastPoint[lastIndex];
                }
                path.Reverse();
                return path;
            }
            

            //attempt to add each connection on the tested point to the queue
            foreach (int connection in testPoint.connections)
            {
                float travelDistance = math.distance(testPoint.transform.position, verticies[connection].transform.position) + pointTravelDistance[testIndex];
                float cost = travelDistance +  calculateHeuristic(verticies, connection, goals);
                //Debug.Log("Point " + connection.ToString() + " has a cost of " + cost.ToString() + " with a travel distance of " + travelDistance.ToString() + " and a heuristic of " + (cost - travelDistance).ToString());
                //if the point has already been tested remove the old point if this path is better or add 
                if (allpoints.Contains(connection))
                {
                    //THERE IS A LOGICAL ERROR HERE, REMOVING ELEMENTS LIKE THIS WILL BREAK THINGS
                    //you can probably keep removing elements if you also decrease the current value or this snippit might be completely unnecessary.
                    //Maybe check how the sudocode deals with duplicate entries,
                    int oldIndex = allpoints.IndexOf(connection);
                    if (totalCost[oldIndex] <= cost)
                    {
                        continue;
                    } 
                    else 
                    {
                        allpoints[oldIndex] = connection;
                        lastPoint[oldIndex] = testIndex;
                        pointTravelDistance[oldIndex] = travelDistance;
                        totalCost[oldIndex] = cost;
                        heap.RemoveObjectAtIndex(oldIndex);
                        heap.Enqueue(cost, oldIndex);
                        continue;
                    }
                }
                //add the connection to the queue and pathScript.points to test out.
                allpoints.Add(connection);
                lastPoint.Add(testIndex);
                pointTravelDistance.Add(travelDistance);
                totalCost.Add(cost);
                heap.Enqueue(cost, currentIndex);
                currentIndex++;
            }
        }

        //its probably set up wrong if the script got to this point. This will happen if there is no way to reach the target avalible. 
        Debug.LogError("Failed to find goal!");

        return new List<int>();
    }

    //calculates the shortest direct distance from a point to a list of possible targets. 
    private float calculateHeuristic(List<hallVertexScript> verticies, int node, List<int> goals)
    {
        //TODO: there is a minor bug where the pathfinding does not seem to consider the players current position when choosing between goal pathScript.points
        //This solution does not work but we only need to come back to it if its an issue
        //float lowestCost = math.distance(verticies[node].transform.position, verticies[goals[0]].transform.position) + math.distance(targetAproximation, verticies[goals[0]].transform.position);
        //for (int i = 1; i < goals.Count; i++)
        //{
        //    float testDistance = math.distance(verticies[node].transform.position, verticies[goals[i]].transform.position) + math.distance(targetAproximation, verticies[goals[i]].transform.position);
        //    lowestCost = math.min(lowestCost, testDistance);
        //}

        return  math.distance(verticies[node].transform.position, targetAproximation);
    }

    //check every connection to see where the closest point is and set the target vertexes and aproximate point.
    public void setTarget(Vector3 targetPosition)
    {
        float shortestDistance = math.INFINITY;
        

        for (int point = 0; point < pathScript.points.Count; point++)
        {
            for (int connection = 0; connection < pathScript.points[point].connections.Count; connection++)
            {
                //skip the connection if all the pathScript.points connections have already been checked (It looks like I removed this, Ill either fix it later or forget about it IDK rn)
                

                //get the closest point on the line
                Vector3 testPoint = closestPointToLineSegment(pathScript.points[point].transform.position, pathScript.points[pathScript.points[point].connections[connection]].transform.position, targetPosition);

                float distanceFromSegment = math.distance(testPoint, targetPosition);

                //set the target vert and shortest distance 
                if (distanceFromSegment < shortestDistance)
                {
                    shortestDistance = distanceFromSegment;
                    targetAproximation = testPoint;
                    pathScript.targetAproximation = testPoint;

                    targetVert1 = point;
                    targetVert2 = pathScript.points[point].connections[connection];
                }
            }
        }
    }

    //This was pulled from a formula on a stack overflow post. Thank you!
    private Vector3 closestPointToLineSegment(Vector3 vert2, Vector3 vert1, Vector3 targetPosition)
    {
        Vector3 AP = targetPosition - vert1;
        Vector3 AB = vert2 - vert1;
        float t = math.dot(AP, AB) / math.dot(AB, AB);

        if (t <= 0)
        {
            return vert1;
        }
        else if (t >= 1)
        {
            return vert2;
        }
        else
        {
            return vert1 + (t * AB);
        }
    }
}
