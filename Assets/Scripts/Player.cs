using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Move Settings")]
    public float maxSpeed = 9f;         // Maximum movement speed
    public float acceleration = 13f;   // Acceleration when moving
    public float decceleration = 16f;  // Deceleration when stopping
    public float velPower = 0.96f;     // Power for velocity smoothing
    public float frictionAmount = 0.2f; // Amount of friction when idle
    private Rigidbody2D rb;            // Rigidbody for movement

    [Header("Dash Settings")]
    public float dashDistance = 5f;    // Distance covered during a dash
    public float dashDuration = 0.2f;  // Duration of the dash
    public float dashCooldown = 0.5f;  // Cooldown time between dashes
    private Vector2 inputDirection;    // Player's input direction
    private bool isDashing = false;    // Is the player currently dashing?
    private float lastDashTime = -Mathf.Infinity; // Last time the player dashed
    private Vector2 lastDashDirection; // Direction of the last dash

    [Header("Attack Settings")]
    public int damage = 20;            // Attack damage
    public float attackSpeed = 0.5f;   // Duration of the attack animation
    public float attackCooldown = 1f; // Cooldown between attacks
    private float lastAttackTime = -Mathf.Infinity; // Last time an attack was made
    private GameObject attackHitbox;   // Reference to the attack hitbox
    private bool isAttacking = false;  // Is the player currently attacking?
    private float attackTimer = 0f;    // Timer for tracking attack animation

    [Header("Health Settings")]
    public float maxHealth = 100f;     // Maximum health
    public float currentHealth;        // Current health

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();                   // Get Rigidbody2D for movement
        currentHealth = maxHealth;                          // Initialize health
        attackHitbox = transform.GetChild(0).gameObject;    // Get the attack hitbox (first child)
    }

    void Update()
    {
        // Handle movement input if not dashing
        if (!isDashing)
        {
            inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (inputDirection.magnitude > 1)
            {
                inputDirection.Normalize();
            }
        }

        // Dash input handling
        if (Input.GetKeyDown(KeyCode.Space) && CanDash())
        {
            StartCoroutine(Dash());
        }

        // Update last dash direction
        if (inputDirection != Vector2.zero)
        {
            lastDashDirection = inputDirection;
        }

        // Attack input handling
        if (Input.GetKeyDown(KeyCode.Mouse0) && CanAttack())
        {
            Attack();
        }

        // Manage attack animation and duration
        if (isAttacking)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer > attackSpeed)
            {
                attackTimer = 0f;
                isAttacking = false;
                attackHitbox.SetActive(isAttacking);
            }
        }
    }

    void FixedUpdate()
    {
        // Apply movement and friction if not dashing
        if (!isDashing)
        {
            ApplyMovement();
            ApplyFriction();
        }
    }

    #region Movement
    private void ApplyMovement()
    {
        Vector2 targetVelocity = inputDirection * maxSpeed;  // Calculate target velocity
        Vector2 velocityDiff = targetVelocity - rb.linearVelocity; // Difference between target and current velocity

        Vector2 movementForce = new Vector2(
            CalculateForce(velocityDiff.x, targetVelocity.x),
            CalculateForce(velocityDiff.y, targetVelocity.y)
        );

        rb.AddForce(movementForce); // Apply calculated movement force
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

            rb.AddForce(-frictionForce, ForceMode2D.Impulse); // Apply friction as an impulse force
        }
    }

    private float CalculateFrictionForce(float velocity)
    {
        return Mathf.Min(Mathf.Abs(velocity), frictionAmount) * Mathf.Sign(velocity);
    }
    #endregion

    #region Dash
    private bool CanDash()
    {
        // Check if enough time has passed since the last dash
        return Time.time >= lastDashTime + dashCooldown;
    }

    private IEnumerator Dash()
    {
        isDashing = true; // Start dashing
        lastDashTime = Time.time; // Record the dash start time

        // Calculate dash velocity
        Vector2 dashDirection = inputDirection == Vector2.zero ? lastDashDirection : inputDirection;
        Vector2 dashVelocity = dashDirection.normalized * (dashDistance / dashDuration);

        rb.linearVelocity = dashVelocity; // Apply dash velocity

        // Wait for the dash duration
        yield return new WaitForSeconds(dashDuration);

        rb.linearVelocity = Vector2.zero; // Stop the dash
        isDashing = false;          // Allow normal movement again
    }
    #endregion

    #region Health
    public void TakeDamage(float damage)
    {
        currentHealth -= damage; // Reduce health by damage amount
    }

    public void HealHealth(float health)
    {
        currentHealth += health; // Increase health by specified amount
    }
    #endregion

    #region Attack
    private void Attack()
    {
        isAttacking = true;                 // Start attack
        attackHitbox.SetActive(isAttacking); // Activate hitbox
    }

    private bool CanAttack()
    {
        return Time.time >= lastAttackTime + attackCooldown; // Check if attack cooldown has passed
    }
    #endregion
}
