using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public class BotController : MonoBehaviour {
    public float speed = 2.0f;
    private int turnJump = 0;
    [Header("Moving Attributes")]
    [SerializeField] private float wallDetectionDistance;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask groundLayer;

    [Header("Attacking Attributes")]
    [SerializeField] private float rushTimeToBubbleX;
    [SerializeField] private float jumpTimeToBubbleY;

    private bool isMovingRight = true;
    private bool isAttacking = false;
    private bool isOnGround = true;
    private Collider2D col;
    private Rigidbody2D rb;

    [SerializeField] List<MonoBehaviour> listAttackBehaviours;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (isAttacking || !isOnGround) return;

        // Move bot in the current direction using Rigidbody2D
        float direction = isMovingRight ? 1 : -1;
        rb.velocity = new Vector2(direction * speed, rb.velocity.y);

        // Check for screen boundaries to reverse direction
        CheckBounds();
    }
    void CheckBounds()
    {
        // Cast a ray in the current moving direction
        float botExtent = col.bounds.extents.x;
        RaycastHit2D hit = Physics2D.Raycast(transform.position,
            isMovingRight ? Vector2.right : Vector2.left,
            wallDetectionDistance + botExtent,
            wallLayer);
        if (hit.collider != null) isMovingRight = !isMovingRight;
    }

    // Patrol with movement and boundary check combined
    private IEnumerator Patrol()
    {
        isAttacking = false;
        float waitTime = 3f; // Default wait time

        // Kiểm tra giá trị của turnJump để điều chỉnh thời gian chờ
        if (turnJump >= 9)
        {
            waitTime = 0f;
        }
        else if (turnJump >= 6)
        {
            waitTime = 1f;
        }
        else if (turnJump >= 3)
        {
            waitTime = 2f;
        }

        if (waitTime == 2)
        {
            if (listAttackBehaviours[0] is IAttackBehavior behavior)
            {
                if (behavior is ShootBallFrozenAttack shootBallFrozen)
                {
                    shootBallFrozen.Angle = 45;
                    shootBallFrozen.ExecuteAttack(this);
                }
            }
        }

        // Wait for 3 seconds before attacking
        yield return new WaitForSeconds(waitTime);
        isAttacking = true; // Start attacking after 3 seconds
        StartCoroutine(RushAndJump());
    }

    // Coroutine thực hiện tấn công vào Bubble
    private IEnumerator RushAndJump()
    {
        List<Bubble> bubbles = BubblesManager.instance.GetBubbleList();
        // Kiểm tra nếu danh sách các bubble có ít nhất một item
        if (bubbles.Count > 0)
        {
            // Lấy một bubble ngẫu nhiên từ danh sách
            Bubble randomBubble = bubbles[Random.Range(0, bubbles.Count)];

            // Rush
            transform.DOMoveX(randomBubble.transform.position.x, rushTimeToBubbleX);
            yield return new WaitForSeconds(rushTimeToBubbleX);

            // Jump
            turnJump++;
            transform.DOMoveY(randomBubble.transform.position.y, jumpTimeToBubbleY);
            yield return new WaitForSeconds(jumpTimeToBubbleY);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isOnGround = true;
            StartCoroutine(Patrol());
        }

        if (collision.gameObject.TryGetComponent(out Bubble bubble))
        {
            bubble.DestroyMyself();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Reset the grounded flag when the bot exits the collision with the ground
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isOnGround = false;
        }
    }
}
