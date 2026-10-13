using UnityEngine;
using UnityEngine.InputSystem;

public class playerControllerScript : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] Transform headTransform;
    [SerializeField] float sensitivity;
    [SerializeField] float maxLookUp;
    [SerializeField] float minLookDown;
    [SerializeField] float gravity;
    [SerializeField] float jumpCooldown;
    private float jumpCooldownTimer;
    private Vector3 input;

    private Vector3 look;

    private Rigidbody rb;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (groundCheck())
        {
            rb.linearVelocity = Quaternion.AngleAxis(headTransform.rotation.eulerAngles.y, Vector3.up) * new Vector3(input.x * speed, rb.linearVelocity.y, input.z * speed);

            if (input.y > 0)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            }
        }
        input.y = 0;
    }

    void FixedUpdate()
    {
        rb.AddForce(Vector3.down * gravity);
    }

    private bool groundCheck()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f, groundMask);
    }
    
    public void Move(InputAction.CallbackContext context) {
        Vector2 movement = context.ReadValue<Vector2>();

        movement = movement.normalized;

        input.x = movement.x;
        input.z = movement.y;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            input.y = 1;
        }
    }

    public void Look(InputAction.CallbackContext context)
    {
        if (enabled){
            Vector2 vec = context.ReadValue<Vector2>();
            vec *= sensitivity;
            look = new Vector3(Mathf.Clamp(look.x - vec.y, minLookDown, maxLookUp), look.y + vec.x, 0);
            headTransform.rotation = Quaternion.Euler(look);
        }
    }
}
