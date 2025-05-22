using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public Animator animator;
    public GameObject attackHitbox;
    private Rigidbody2D rb;
    private bool isGrounded;

    private bool isDead = false;

    // NEW: Attack cooldown variables
    public float attackCooldown = 0.5f;
    private float lastAttackTime = -999f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        if (isDead) return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        float move = Input.GetAxis("Horizontal");
        transform.Translate(Vector2.right * move * moveSpeed * Time.deltaTime);

        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -9f, 9f);
        transform.position = clampedPosition;

        animator.SetBool("isMoving", Mathf.Abs(move) > 0.01f);

        if (move != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(move) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetTrigger("Jump");
        }

        // Attack only if cooldown has passed
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }
    }

    void Attack()
    {
        lastAttackTime = Time.time; // ✅ Record time of attack

        animator.SetTrigger("Attack");

        if (attackHitbox != null)
        {
            attackHitbox.SetActive(true);
            Invoke("DisableHitbox", 0.3f);
        }
    }

    void DisableHitbox()
    {
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false);
        }
    }

    public void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        GameManeger.Instance.GameOver();
    }
}
