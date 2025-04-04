using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


// TODO: Finish refactoring
public class Player : MonoBehaviour
{
    #region Move variables
    [Header("Move Settings")]
    public float maxSpeed = 9f;         // Maximum movement speed
    public float acceleration = 13f;    // Acceleration when moving
    public float decceleration = 16f;   // Deceleration when stopping
    public float velPower = 0.96f;      // Power for velocity smoothing
    public float frictionAmount = 0.2f; // Amount of friction when idle
    private Rigidbody2D rb;             // Rigidbody for movement
    #endregion

    #region Dash variables
    [Header("Dash Settings")]
    public float dashDistance = 5f;                     // Distance covered during a dash
    public float dashDuration = 0.2f;                   // Duration of the dash
    public float dashCooldown = 0.5f;                   // Cooldown time between dashes
    private Vector2 inputDirection;                     // Player's input direction
    private float lastDashTime = -Mathf.Infinity;       // Last time the player dashed
    private Vector2 currDirection = new Vector2(1, 0);  // Direction of the last dash
    #endregion

    #region Attack variables
    [Header("Attack Settings")]
    public int damage = 20;                         // Attack damage
    public float attackSpeed = 0.5f;                // Maximum number of attacks parformed per second
    public float attackDuration = 0.1f;             // Duration of the attack animation
    private float lastAttackTime = -Mathf.Infinity; // Last time an attack was made
    private GameObject attackHitbox;                // Reference to the attack hitbox
    private bool isAttacking = false;               // Is the player currently attacking?
    private float attackTimer = 0f;                 // Timer for tracking attack animation
    #endregion

    #region Health variables
    [Header("Health Settings")]
    public float maxHealth = 100f;  // Maximum health
    public float currentHealth;     // Current health
    #endregion

    private PlayerStateMachine psm = new PlayerStateMachine();
    public MainActionMap playerActions = new MainActionMap();
    void Awake()
    {
        playerActions.Player.Enable();
        Debug.Log("Awaken");
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();                   // Get Rigidbody2D for movement
        currentHealth = maxHealth;                          // Initialize health
        attackHitbox = transform.GetChild(0).gameObject;    // Get the attack hitbox (first child)
    }

    void FixedUpdate()
    {
        ExecuteMovement();
    }

    void Update()
    {
        inputDirection = playerActions.Player.Move.ReadValue<Vector2>();
        
        if (inputDirection != Vector2.zero)
        {
            currDirection = inputDirection;
        }
    }

    // void Update()
    // {

//         //  Attack input handling
//         if (Input.GetKeyDown(KeyCode.Mouse0) && CanAttack())
//         {
//             Attack();
//    lastAttackTime = Time.time;
//         }

////  Manage attack animation and duration
//if (isAttacking)
//{
//    attackTimer += Time.deltaTime;

//    if (attackTimer > 1 / attackSpeed)
//    {
//        attackTimer = 0f;
//        isAttacking = false;
//        attackHitbox.SetActive(isAttacking);
//    }
//}
//     }


    #region Movement

    private float CalculateFrictionForce(float velocity)
    {
        return Mathf.Min(Mathf.Abs(velocity), frictionAmount) * Mathf.Sign(velocity);
    }

    private float CalculateForce(float speedDiff, float targetSpeed)
    {
        float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : decceleration;
        return Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, velPower) * Mathf.Sign(speedDiff);
    }
    
    private void ApplyMovingForce()
    {
        Vector2 targetVelocity = inputDirection * maxSpeed;         // Calculate target velocity
        Vector2 velocityDiff = targetVelocity - rb.linearVelocity;  // Difference between target and current velocity

        Vector2 movementForce = new Vector2(
            CalculateForce(velocityDiff.x, targetVelocity.x),
            CalculateForce(velocityDiff.y, targetVelocity.y)
        );

        rb.AddForce(movementForce); // Apply calculated movement force
    }

    private void ApplyFrictionForce()
    {
        Vector2 frictionForce = new Vector2(
            CalculateFrictionForce(rb.linearVelocity.x),
            CalculateFrictionForce(rb.linearVelocity.y)
        );

        rb.AddForce(-frictionForce, ForceMode2D.Impulse);   // Apply friction as an impulse force
    }

    public void ExecuteMovement()
    {
        ApplyMovingForce();
        ApplyFrictionForce();
    }

    #endregion

    #region Dash
    private bool CanDash()
    {
        // Check if enough time has passed since the last dash
        return Time.time >= lastDashTime + dashCooldown;
    }

    private IEnumerator PerformDash()
    {
        lastDashTime = Time.time;   // Record the dash start time

        // Calculate dash velocity
        Vector2 dashDirection = inputDirection == Vector2.zero ? currDirection : inputDirection;
        Vector2 dashVelocity = dashDirection.normalized * (dashDistance / dashDuration);

        rb.linearVelocity = dashVelocity;   // Apply dash velocity

        // Wait for the dash duration
        yield return new WaitForSeconds(dashDuration);

        rb.linearVelocity = Vector2.zero;   // Stop the dash
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (CanDash() && context.performed)
        {
            Debug.Log("dashed!");
            StartCoroutine(PerformDash());
        }
    }

    #endregion
    
    #region Health
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;    // Reduce health by damage amount
    }

    public void HealHealth(float health)
    {
        currentHealth += health;    // Increase health by specified amount
    }
    #endregion

    #region Attack


    public IEnumerator PerformAttack()
    {
        isAttacking = true;
        attackHitbox.SetActive(true);

        yield return new WaitForSeconds(attackDuration); // Attack duration

        isAttacking = false;
        attackHitbox.SetActive(false);
        lastAttackTime = Time.time;
    }

    private bool CanAttack()
    {
        return Time.time >= lastAttackTime + (1f / attackSpeed);    // Check if attack cooldown has passed
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (CanAttack() && context.performed)
        {
            Debug.Log("attacked!");
            StartCoroutine(PerformAttack());
        }
    }
    #endregion
}
