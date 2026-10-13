using System.Collections.Generic;
using UnityEngine;

public class EyeRotateScript : MonoBehaviour
{
    [SerializeField] List<Transform> eyes;
    [SerializeField] Transform player;

    void Update()
    {
        foreach (Transform eye in eyes)
        {
            eye.LookAt(player, Vector3.up);
            eye.rotation *= Quaternion.AngleAxis(90, Vector3.right);
            //Hello
        }
    }
}
