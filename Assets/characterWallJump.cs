using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterWallJump : MonoBehaviour
{
    public float speed;
    Rigidbody rb;

    Camera cam;
    public float sensitivity;
    public float yRotSpeed = 10;
    public float jumpForce = 100;
    public Vector2 wallJumpForce;
    public float speedThresshold = 1;
    public float gravityForce;
    public float wallrunSpeedMultiplier = 2;
    public float wallrunningGravityForce;
    public Transform groundCheckObj;

    bool is_pressing_space = false;
    bool was_pressing_space = false;
    public LayerMask wallMask;

    bool was_wallrunning = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
    }

    private void Update()
    {
        is_pressing_space = Input.GetKey(KeyCode.Space);
    }

    private void FixedUpdate()
    {
        bool space_press = is_pressing_space && !was_pressing_space;
        // Movement
        Vector2 xMov = new Vector2(Input.GetAxisRaw("Horizontal") * transform.right.x, Input.GetAxisRaw("Horizontal") * transform.right.z);
        Vector2 zMov = new Vector2(Input.GetAxisRaw("Vertical") * transform.forward.x, Input.GetAxisRaw("Vertical") * transform.forward.z);

        Vector2 velocity = (xMov + zMov).normalized * speed * Time.fixedDeltaTime;

        // Rotation
        float yRot = Input.GetAxisRaw("Mouse X") * sensitivity * yRotSpeed;
        rb.rotation *= Quaternion.Euler(0, yRot * Time.fixedDeltaTime, 0);

        float xRot = Input.GetAxisRaw("Mouse Y") * sensitivity;
        cam.transform.rotation *= Quaternion.Euler(-xRot, 0, 0);


        // Wall jump check
        RaycastHit[] leftHits = Physics.RaycastAll(transform.position, -transform.right, 1);
        RaycastHit[] rightHits = Physics.RaycastAll(transform.position, transform.right, 1);
        bool wallOnLeft = leftHits.Length > 0;
        bool wallOnRight = rightHits.Length > 0;

        bool is_wallrunning;

        float _speed = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;

        bool is_grounded = Physics.CheckSphere(groundCheckObj.transform.position, 0.04f);
        if ((wallOnLeft || wallOnRight) && !is_grounded)
        {
            is_wallrunning = true;
            if (!was_wallrunning)
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z) * wallrunSpeedMultiplier;
        }
        else
        {
            is_wallrunning = false;
        }
        
        if (!is_wallrunning && is_grounded)
            rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.y);
        
        // Jumping
        if (space_press)
        {
            if (is_grounded)
            {
                Jump();
            }
            if (is_wallrunning)
            {
                if (wallOnLeft)
                    WallJump(leftHits[0].point - transform.position);
                if (wallOnRight)
                    WallJump(rightHits[0].point - transform.position);
            }
        }

        rb.AddForce(Vector3.down * (is_wallrunning && _speed > speedThresshold ? wallrunningGravityForce : gravityForce));

        was_wallrunning = is_wallrunning;
        was_pressing_space = is_pressing_space;
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce);
    }

    void WallJump(Vector3 dir)
    {
        rb.AddForce((Vector3.up * wallJumpForce.y - dir.normalized * wallJumpForce.x));
    }
}
