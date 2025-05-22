using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Animator animator;
    public float moveSpeed = 2f;
    public float chaseRange = 10f;

    private Transform player;
    private bool isDead = false;
    private bool isAttacking = false;
    private float originalScaleX;

    // Hitpoint variables
    private int hitPoints;
    private int currentHits = 0;

    // Attack cooldown variables
    private float attackCooldown = 1.0f;
    private float lastAttackTime = 0f;

    // Hit cooldown (to prevent multiple hits from same attack)
    private float lastHitTime = 0f;
    private float hitCooldown = 0.4f;

    private void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        animator.Play("Enemy_idle");

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        originalScaleX = transform.localScale.x;

        // Set hitPoints based on level
        hitPoints = Mathf.Max(1, GameManeger.Instance != null ? GameManeger.Instance.currentlevel : 1);
    }

    private void Update()
    {
        if (isDead || isAttacking || player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < chaseRange)
        {
            Vector2 targetPosition = new Vector2(player.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            animator.SetBool("Enemy_move", true);

            // Flip to face player
            if (player.position.x < transform.position.x)
                transform.localScale = new Vector3(Mathf.Abs(originalScaleX), transform.localScale.y, transform.localScale.z);
            else
                transform.localScale = new Vector3(-Mathf.Abs(originalScaleX), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            animator.SetBool("Enemy_move", false);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (isDead) return;

        // Enemy attacks player on contact with cooldown
        if (other.CompareTag("Player") && Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            StartCoroutine(AttackPlayer());
        }

        // Enemy takes damage from player attack only if cooldown has passed
        if (other.CompareTag("Attack") && Time.time >= lastHitTime + hitCooldown)
        {
            lastHitTime = Time.time;
            currentHits++;

            if (currentHits >= hitPoints)
            {
                StartCoroutine(Die());
            }
            else
            {
                animator.SetTrigger("Enemy_hit");
            }
        }
    }

    private IEnumerator AttackPlayer()
    {
        isAttacking = true;
        animator.SetBool("Enemy_move", false);
        animator.SetTrigger("Enemy_attack");

        yield return new WaitForSeconds(0.5f); // Match with attack animation timing

        if (isDead) yield break;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            PlayerHealth health = playerObj.GetComponent<PlayerHealth>();
            if (health != null)
            {
                int damageAmount = 1;
                int currentLevel = GameManeger.Instance != null ? GameManeger.Instance.currentlevel : 1;

                if (currentLevel >= 4 && currentLevel <= 5)
                {
                    damageAmount = 2;
                }

                health.TakeDamage(damageAmount);
            }
        }

        isAttacking = false;
    }

    private IEnumerator Die()
    {
        isDead = true;
        animator.SetBool("Enemy_move", false);
        animator.SetTrigger("Enemy_die");

        yield return new WaitForSeconds(0.6f);
        NextLevel();
        Destroy(gameObject, 0.5f);
    }

    private void NextLevel()
    {
        if (GameManeger.Instance != null)
        {
            GameManeger.Instance.LoadNextLevel();
        }
        else
        {
            Debug.LogError("GameManeger.Instance is null! Make sure GameManeger exists in the scene.");
        }
    }
}
