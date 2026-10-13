using Unity.Mathematics;
using UnityEngine;

public class IKsystem : MonoBehaviour
{
    [SerializeField] float greediness;
    [SerializeField] Transform target;
    [SerializeField] int chainLength;
    [SerializeField] int iterations;
    [SerializeField] Transform hint;
    // Update is called once per frame
    void Update()
    {
        Transform boneToReset = transform.parent;
        for (int n = 0; n < chainLength; n++)
        {
            boneToReset.localRotation = Quaternion.identity;
            boneToReset = boneToReset.parent;
        }
        for (int i = 0; i < iterations; i++)
        {
            Transform bone = transform.parent;

            for (int n = 0; n < chainLength; n++)
            {
                if (bone == null)
                    break;

                Vector3 toEnd = transform.position - bone.position;
                Vector3 toTarget = target.position - bone.position;

                float angle = Vector3.Angle(toEnd, toTarget);
                Vector3 axis = Vector3.Cross(toEnd, toTarget);

                if (axis.sqrMagnitude > 0.000001f)
                {
                    axis.Normalize();

                    Quaternion delta =
                        Quaternion.AngleAxis(
                            angle * greediness,
                            axis
                        );

                    bone.rotation = delta * bone.rotation;
                }

                bone = bone.parent;
            }
        }
    }


}
