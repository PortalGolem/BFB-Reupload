using UnityEngine;
using UnityEngine.Splines;

public class SplineMover : MonoBehaviour
{
    [SerializeField] SplineContainer splineContainer;
    [SerializeField] float speed = .01f;
    private float splineProgress;
    private Spline currentSpline;

    private void Start()
    {
        Spline spline = splineContainer[1];
        moveAlongSpline(1, spline);
    }

    // Update is called once per frame
    void Update()
    {
        moveAlongSpline(speed * Time.deltaTime, splineContainer.Spline);
       
    }


    //move the object a distance along a spline
    public double moveAlongSpline(float distance, Spline spline) {
        float length = spline.GetLength();
        splineProgress += distance;
        if (splineProgress - length > 0)
        {
            transform.position = spline.EvaluatePosition(1.0f);
            return splineProgress - length;
        }
        else
        {
            transform.position = spline.EvaluatePosition(splineProgress / length);
        }

        return 0;
    }
}
