using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MoveAlongPath : MonoBehaviour
{
    [SerializeField] private int startingPoint;
    [SerializeField] Transform modelTransform;
    [SerializeField] private float modelMoveSpeed;
    [SerializeField] private float maxModelLagDistance;
    private Vector3 targetPosition;
    private List<int> path = new List<int> { 0 };
    private int currentPoint = 0;
    private HallTraverserScript hallTraverserScript;
    private Vector3 lastPoint = Vector3.zero;

    //This is a testement to poorly commented code.

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hallTraverserScript = gameObject.GetComponent<HallTraverserScript>();
        transform.position = hallTraverserScript.pathScript.points[startingPoint].transform.position;
    }

    public void setTarget(Vector3 target)
    {
        bool isAtPlayer = currentPoint >= path.Count;
        path = hallTraverserScript.GenerateAstar(target, path[isAtPlayer ? currentPoint - 1 : currentPoint]);
        currentPoint = isAtPlayer ? 1 : 0;
        targetPosition = hallTraverserScript.targetAproximation;
    }

    public float MoveAlong(float distance)
    {
        if (currentPoint == path.Count)
        {
            float distanceToTarget = math.distance(transform.position, targetPosition);
            if (distance > distanceToTarget)
            {
                transform.position = targetPosition;
                modelTransform.rotation = Quaternion.LookRotation(modelTransform.position - lastPoint + transform.forward * .001f, transform.up);
                lastPoint = modelTransform.position;
                float modelDistance = Vector3.Distance(modelTransform.position, transform.position);
                modelTransform.position += (transform.position - modelTransform.position).normalized * math.min(Time.deltaTime * modelMoveSpeed, modelDistance);
                if (modelDistance > maxModelLagDistance)
                {
                    modelTransform.position = (modelTransform.position - transform.position).normalized * maxModelLagDistance + transform.position;
                }
                return distance - distanceToTarget;
            }
            else
            {
                transform.position = transform.position + (distance * (targetPosition - transform.position).normalized);
                modelTransform.rotation = Quaternion.LookRotation(modelTransform.position - lastPoint + transform.forward * .0001f, transform.up);
                lastPoint = modelTransform.position;
                float modelDistance = Vector3.Distance(modelTransform.position, transform.position);
                modelTransform.position += (transform.position - modelTransform.position).normalized * math.min(Time.deltaTime * modelMoveSpeed, modelDistance);
                if (modelDistance > maxModelLagDistance)
                {
                    modelTransform.position = (modelTransform.position - transform.position).normalized * maxModelLagDistance + transform.position;
                }
                return 0;
            }
        }
        else
        {
            float distanceToPoint = math.distance(transform.position, hallTraverserScript.pathScript.points[path[currentPoint]].transform.position);
            if (distance >= distanceToPoint)
            {
                transform.position = hallTraverserScript.pathScript.points[path[currentPoint]].transform.position;
                currentPoint++;
                return MoveAlong(distance - distanceToPoint);
            }
            else
            {
                transform.position = transform.position + (distance * (hallTraverserScript.pathScript.points[path[currentPoint]].transform.position - transform.position).normalized);
                modelTransform.rotation = Quaternion.LookRotation(modelTransform.position - lastPoint + transform.forward * .0001f, transform.up);
                lastPoint = modelTransform.position;
                float modelDistance = Vector3.Distance(modelTransform.position, transform.position);
                modelTransform.position += (transform.position - modelTransform.position).normalized * math.min(Time.deltaTime * modelMoveSpeed, modelDistance);
                if (modelDistance > maxModelLagDistance)
                {
                    modelTransform.position = (modelTransform.position - transform.position).normalized * maxModelLagDistance + transform.position;
                }
                return 0;
            }
        }
    }
}
