using System.Collections;
using UnityEngine;

public class RigidBodyController : MonoBehaviour
{
    [Header("Move Settings")]
    public float maxSpeed = 9f;
    public float acceleration = 13f;
    public float decceleration = 16f;
    public float velPower = 0.96f;
    public float frictionAmount = 0.2f;

    [Header("Dash Settings")]
    [Header("Dash Settings")]
    public float dashDistance = 5f; // Total distance to dash
    public float dashDuration = 0.2f; // Time taken to dash the distance
    public float dashCooldown = 0.5f; // Cooldown time between dashes

    private Rigidbody2D rb;
    private Vector2 inputDirection;
    private bool isDashing = false;
    private float lastDashTime = -Mathf.Infinity;
    private Vector2 lastDashDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Capture input direction if not dashing
        if (!isDashing)
        {
            inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (inputDirection.magnitude > 1)
            {
                inputDirection.Normalize();
            }
        }

        // Handle dash input
        if (Input.GetKeyDown(KeyCode.Space) && CanDash())
        {
            StartCoroutine(Dash());
        }

        // Update last dash direction
        if (inputDirection != Vector2.zero)
            lastDashDirection = inputDirection;
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            ApplyMovement();
            ApplyFriction();
        }
    }

    private void ApplyMovement()
    {
        Vector2 targetVelocity = inputDirection * maxSpeed;
        Vector2 velocityDiff = targetVelocity - rb.linearVelocity;

        Vector2 movementForce = new Vector2(
            CalculateForce(velocityDiff.x, targetVelocity.x),
            CalculateForce(velocityDiff.y, targetVelocity.y)
        );

        rb.AddForce(movementForce);
    }

    private float CalculateForce(float speedDiff, float targetSpeed)
    {
        float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : decceleration;
        return Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, velPower) * Mathf.Sign(speedDiff);
    }

    private void ApplyFriction()
    {
        if (inputDirection == Vector2.zero)
        {
            Vector2 frictionForce = new Vector2(
                CalculateFrictionForce(rb.linearVelocity.x),
                CalculateFrictionForce(rb.linearVelocity.y)
            );

            rb.AddForce(-frictionForce, ForceMode2D.Impulse);
        }
    }

    private float CalculateFrictionForce(float velocity)
    {
        return Mathf.Min(Mathf.Abs(velocity), frictionAmount) * Mathf.Sign(velocity);
    }

    private bool CanDash()
    {
        // Check if enough time has passed since the last dash
        return Time.time >= lastDashTime + dashCooldown;
    }

    private IEnumerator Dash()
    {
        isDashing = true; // Start dashing

        // Update the last dash time for cooldown
        lastDashTime = Time.time;

        // Calculate the dash velocity based on the desired distance and duration
        Vector2 dashDirection = inputDirection == Vector2.zero ? lastDashDirection : inputDirection;
        Vector2 dashVelocity = dashDirection.normalized * (dashDistance / dashDuration);

        // Set the Rigidbody velocity for the dash
        rb.linearVelocity = dashVelocity;

        // Wait for the dash duration
        yield return new WaitForSeconds(dashDuration);

        // Stop the dash and reset the velocity
        rb.linearVelocity = Vector2.zero;
        isDashing = false; // Allow normal movement again
    }

}