using UnityEngine;

public class BodyController : MonoBehaviour
{
    public Transform groundCheck;
    public LayerMask groundMask;

    private Rigidbody rb;
    
    public bool isGrounded, isWalking;
    //public Animator camAnim;
    public float groundDamping = 1f;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private int jumpCount = 0;

    private float mass;

    private CharacterStats charStats;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        charStats = GetComponent<CharacterStats>();
    }

    private void Update()
    {
        //camAnim.SetBool("isWalking", isWalking);

        //Check if the player is grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.1f, groundMask);
        //isWallJumping = Physics.CheckSphere(groundCheck.position, 0.3f, groundMask);

        //Reset jump count when grounded
        if (isGrounded) { jumpCount = 0; }

        //Movement
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        //Convert move direction to world space
        moveDirection = transform.TransformDirection(moveDirection);

        //Apply gravity force
        if (!isGrounded)
        {
            rb.AddForce(Vector3.down * charStats.gravityForce, ForceMode.Acceleration);
        }

        //Multi-Jump Logic
        if (Input.GetButtonDown("Jump") && jumpCount < charStats.maxJumpCount)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);  //Reset vertical velocity
            rb.AddForce(Vector3.up * charStats.jumpForce, ForceMode.Impulse);
            jumpCount++;
        }

        //Rotate player based on mouse input
        float mouseX = Input.GetAxis("Mouse X");
        Vector3 rotation = new Vector3(0f, mouseX * charStats.rotationSpeed, 0f);
        transform.Rotate(rotation);

        if (rb.linearVelocity.magnitude > 3f)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }

    }

    private void FixedUpdate()
    {
        //Apply movement force
        if (moveDirection != Vector3.zero)
        {
            if (isGrounded)
            { rb.linearVelocity *= Mathf.Pow(1f - groundDamping * Time.deltaTime, 1f); }

            Vector3 moveForce = moveDirection * charStats.moveSpeed * Time.deltaTime;
            rb.AddForce(moveForce, ForceMode.VelocityChange);
        }
        else if (isGrounded) //Apply ground damping when no input and grounded
        {
            rb.linearVelocity *= Mathf.Pow(1f - groundDamping * 5f * Time.deltaTime, 2f);
        }



        //Clamp max velocity, but scale it with moveSpeed
        float baseMaxSpeed = 8f;
        float maxSpeed = baseMaxSpeed + (charStats.moveSpeed * 0.5f);

        //Don't clamp Y so jumping/gravity still feel right
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (horizontalVelocity.magnitude > maxSpeed)
        {
            Vector3 limitedVelocity = horizontalVelocity.normalized * maxSpeed;
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z), 0.2f);
        }

        
    }


}