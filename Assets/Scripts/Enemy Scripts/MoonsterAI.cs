using UnityEngine;

public class MoonsterAI : MonoBehaviour
{
    [SerializeField] Transform playerTransform;
    [SerializeField] float speed;
    private MoveAlongPath moveAlongPath;
    void Start()
    {
        moveAlongPath = gameObject.GetComponent<MoveAlongPath>();
    }
    //This behavior simply constantly targets the player's position and moves towards the target. This can be replaced with more complicated rules and behaviors such as line of sight, patrol paterns, or other things.
    void Update()
    {
        moveAlongPath.setTarget(playerTransform.position);
        moveAlongPath.MoveAlong(speed * Time.deltaTime);
    }
}
