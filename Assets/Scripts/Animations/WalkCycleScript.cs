using System.Collections.Generic;
using UnityEngine;

public class WalkCycleScript : MonoBehaviour
{
    [SerializeField] List<Transform> legTargets;
    [SerializeField] List<Transform> footPositions;
    [SerializeField] Transform bodyPosition;
    [SerializeField] List<Vector3> footOffsets;
    [SerializeField] float targetOffset;
    [SerializeField] float surfaceCheckDistance = 10f;
    [SerializeField] LayerMask floorMask;
    [SerializeField] AnimationCurve stepHeightCurve;
    [SerializeField] AnimationCurve stepSpeedCurve;
    [SerializeField] float stepHeightMultiplier;
    [SerializeField] float stepTimeMax;
    [SerializeField] float stepDistanceMax;
    [SerializeField] Vector3 finalFootOffset;
    private List<Vector3> targetPositions = new List<Vector3>();
    [SerializeField] List<bool> isFootGroupOne = new List<bool>();
    [SerializeField] Vector3 bodyOffset = Vector3.zero;

    private List<Quaternion> bodyTargetRotations = new List<Quaternion>();

    private List<float> footProgress = new List<float>();
    private List<Vector3> currentNormalGoal = new List<Vector3>();
    private List<Vector3> currentFootGoal = new List<Vector3>();
    private List<bool> isFootMoving = new List<bool>();
    private List<Vector3> footStartPositions = new List<Vector3>();
    private List<Vector3> targetNormals = new List<Vector3>();
    private List<Vector3> currentNormals = new List<Vector3>();
    private List<Quaternion> footRotation = new List<Quaternion>();
    private bool isGroupOneMoving = true;
    float currentStepTime;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < legTargets.Count; i++)
        {
            targetPositions.Add(legTargets[i].position);
            footProgress.Add(0);
            currentFootGoal.Add(Vector3.zero);
            footStartPositions.Add(Vector3.zero);
            targetNormals.Add(Vector3.up);
            currentNormals.Add(Vector3.up);
            currentNormalGoal.Add(Vector3.up);
            footRotation.Add(Quaternion.identity);
            bodyTargetRotations.Add(Quaternion.identity);
            if (isFootGroupOne.Count < legTargets.Count)
            {
                Debug.LogError("Missing foot group identifier.");
                continue;
            }
            isFootMoving.Add(isFootGroupOne[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {

        //Update all of the target positions to valid surfaces on nearby colliders.

        for (int i = 0; i < targetPositions.Count; i++)
        {
            //calculate the "default" target positions
            targetPositions[i] = transform.position + (transform.rotation * (footOffsets[i] + new Vector3(0, 0, targetOffset)));

            //Find all nearby colliders and find the most suitable point on them for a target.
            Collider[] colliders = Physics.OverlapSphere(targetPositions[i], surfaceCheckDistance, floorMask, QueryTriggerInteraction.Ignore);
            Vector3 newTarget = Vector3.positiveInfinity;
            RaycastHit hit;
            float distanceToNewTarget = Mathf.Infinity;
            for (int n = 0; n < colliders.Length; n++)
            {
                Vector3 testPoint = colliders[n].ClosestPoint(targetPositions[i]);
                float distanceToTestPoint = Vector3.Distance(targetPositions[i], testPoint);
                if (distanceToTestPoint < distanceToNewTarget)
                {
                    if (testPoint - targetPositions[i] != Vector3.zero)
                    {
                        Ray ray = new Ray(targetPositions[i], (testPoint - targetPositions[i]).normalized);
                        if (colliders[n].Raycast(ray, out hit, Mathf.Infinity))
                        {
                            targetNormals[i] = hit.normal;
                        }
                    }
                    distanceToNewTarget = distanceToTestPoint;
                    newTarget = testPoint;
                }
            }
            if (distanceToNewTarget < surfaceCheckDistance)
            {
                targetPositions[i] = newTarget;
            }
        }


        // decide which feet should be moving

        bool isLastGroupFinished = true;

        for (int i = 0; i < isFootMoving.Count; i++)
        {
            if (isGroupOneMoving == isFootGroupOne[i] && isFootMoving[i])
            {
                isLastGroupFinished = false;
            }
        }

        if (isLastGroupFinished)
        {
            isGroupOneMoving = !isGroupOneMoving;
            for (int i = 0; i < isFootMoving.Count; i++)
            {
                if (isFootGroupOne[i] == isGroupOneMoving)
                {
                    //Set goals and progress for next step
                    isFootMoving[i] = true;
                    currentNormals[i] = currentNormalGoal[i];
                    currentFootGoal[i] = targetPositions[i];
                    currentNormalGoal[i] = targetNormals[i];
                    currentStepTime = 0;
                    footProgress[i] = 0;
                    footStartPositions[i] = legTargets[i].transform.position;
                }
            }
        }



        currentStepTime += Time.deltaTime;


        for (int i = 0; i < currentFootGoal.Count; i++)
        {
            // Calculate the current progress
            float distanceProgress = 0;
            if (!isFootMoving[i])
            {
                footPositions[i].rotation = footRotation[i];
                continue;
            }

            float distance = Vector3.Distance(currentFootGoal[i], targetPositions[i]);
            distance /= stepDistanceMax;
            if (distance > distanceProgress)
            {
                distanceProgress = distance;
            }

            footProgress[i] = Mathf.Max(currentStepTime / stepTimeMax, distanceProgress);

            if (footProgress[i] >= 1f)
            {
                isFootMoving[i] = false;
                footProgress[i] = 1;
            }

            // Calculate new foot position

            Vector3 up = Vector3.Lerp(currentNormals[i], currentNormalGoal[i], stepSpeedCurve.Evaluate(footProgress[i]));
            legTargets[i].position = Vector3.Lerp(footStartPositions[i], currentFootGoal[i], stepSpeedCurve.Evaluate(footProgress[i])) + Quaternion.LookRotation(up) * finalFootOffset + (up * stepHeightCurve.Evaluate(stepSpeedCurve.Evaluate(footProgress[i])));
            footPositions[i].rotation = Quaternion.LookRotation(up, Vector3.up) * Quaternion.AngleAxis(90, Vector3.left);
            bodyTargetRotations[i] = footPositions[i].rotation;
            footRotation[i] = footPositions[i].rotation;
        }
        Quaternion newBodyRotation = calcAvg(bodyTargetRotations) * Quaternion.AngleAxis(180, Vector3.right);
        bodyPosition.localRotation = Quaternion.Euler(-newBodyRotation.eulerAngles.x, bodyPosition.localRotation.eulerAngles.y, -newBodyRotation.eulerAngles.z);
        bodyPosition.position = transform.parent.position + Quaternion.Inverse(bodyPosition.rotation) * bodyOffset;
    }


    //Credit to stack overflow user sam hocevar Thanks!
    private Quaternion calcAvg(List<Quaternion> rotationlist)
    {
        float x = 0, y = 0, z = 0, w = 0;
        foreach (Quaternion go in rotationlist)
        {
            Quaternion q = go;
            x += q.x; y += q.y; z += q.z; w += q.w;
        }
        float k = 1.0f / Mathf.Sqrt(x * x + y * y + z * z + w * w);
        return new Quaternion(x * k, y * k, z * k, w * k);
    }


    void OnDrawGizmos()
    {
        foreach (Vector3 pos in targetPositions)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(pos, .1f);
        }
        foreach (Vector3 pos in currentFootGoal)
        {
            Gizmos.color = Color.ivory;
            Gizmos.DrawSphere(pos, .1f);
        }
        foreach (Vector3 offset in footOffsets)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position + offset, .1f);
        }
    }
}
