using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PriorityQueue
{
    public List<QueueObject> queue = new();

    public QueueObject Dequeue()
    {
        //remove and return the next element in the queue
        int lastIndex = queue.Count - 1;
        QueueObject i = queue[lastIndex];
        queue.RemoveAt(lastIndex);
        return i;
    }

    public void Enqueue(float cost, int index)
    {
        //Instert an element into the queue based off of its cost value
        //with lower cost itmes on the end.
        QueueObject q = new QueueObject();
        q.index = index;
        q.cost = cost;
        for (int i = 0; i < queue.Count - 1; i++)
        {
            if (cost <= queue[i].cost && cost >= queue[i + 1].cost)
            {
                queue.Insert(i + 1, q);
                return;
            }
        }
        queue.Insert(0, q);
        return;
    }

    public QueueObject Peek()
    {
        //Return the next element without removing it
        return queue[^1];
    }
    public void RemoveObjectAtIndex(int i)
    {
        for (int o = 0; o < queue.Count; o++)
        {
            if (queue[o].index == i)
            {
                queue.RemoveAt(o);
            }
        }
    }

    override public string ToString()
    {
        string outputString = "{";
        for (int i = 0; i < queue.Count; i++)
        {
            outputString = outputString + "{" + queue[i].cost + ": " + queue[i].index + "}, ";
        }
        return outputString + "}";
    }
}

public struct QueueObject
{
    public int index;
    public float cost;
}