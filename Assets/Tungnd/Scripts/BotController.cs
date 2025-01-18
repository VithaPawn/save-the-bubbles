using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public class BotController : MonoBehaviour
{
    BubbleManager bubbleManager;
    public float speed = 2.0f; // Tốc độ di chuyển của bot
    private bool movingRight = true; // Hướng di chuyển
    private Camera mainCamera; // Camera chính
    private Collider2D col;
    private Rigidbody2D rb;
    private bool isAttacking = false;
    private bool isOnGround = true;

    public float jumpForce = 5.0f; // Lực nhảy
    private int turnJump = 0;

    [SerializeField] List<MonoBehaviour> listAttackBehaviours;

    void Start()
    {
        bubbleManager = FindObjectOfType<BubbleManager>();
        rb = GetComponent<Rigidbody2D>();   
        mainCamera = Camera.main; // Gán Camera chính
        col = GetComponent<Collider2D>(); // Lấy Collider2D      
    }

    private void Update()
    {       
        if (isAttacking || !isOnGround) return;

        // Move bot in the current direction using Rigidbody2D
        float direction = movingRight ? 1 : -1;
        rb.velocity = new Vector2(direction * speed, rb.velocity.y); // Apply velocity only in the X direction

        // Check for screen boundaries to reverse direction
        CheckBounds();
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

        if(waitTime == 0)
        {
            if (listAttackBehaviours[0] is IAttackBehavior behavior)
            {
                if(behavior is ShootBallFrozenAttack shootBallFrozen)
                {
                    shootBallFrozen.Angle = 45;
                    shootBallFrozen.ExecuteAttack(this);
                    shootBallFrozen.Angle = 135;
                    shootBallFrozen.ExecuteAttack(this);
                }
                
            }
        }
              
        // Wait for 3 seconds before attacking
        yield return new WaitForSeconds(waitTime);
        isAttacking = true; // Start attacking after 3 seconds
        StartCoroutine(JumpToEatBubble());
    }

    void CheckBounds()
    {
        // Get the current bot bounds in the world space
        Vector3 botLeft = transform.position + col.bounds.min - col.bounds.center;
        Vector3 botRight = transform.position + col.bounds.max - col.bounds.center;

        // Convert world space positions to screen space
        Vector3 screenLeft = mainCamera.WorldToScreenPoint(botLeft);
        Vector3 screenRight = mainCamera.WorldToScreenPoint(botRight);

        // Reverse direction when bot hits the screen edges
        if (screenLeft.x <= 0 + 10 && !movingRight)
        {
            movingRight = true;
        }
        else if (screenRight.x >= Screen.width - 10 && movingRight)
        {
            movingRight = false;
        }
    }

    // Coroutine thực hiện tấn công vào Bubble
    private IEnumerator JumpToEatBubble()
    {
        // Kiểm tra nếu danh sách các bubble có ít nhất một item
        if (bubbleManager.listManager.Count > 0)
        {
            // Lấy một bubble ngẫu nhiên từ danh sách
            BubbleTungnd randomBubble = bubbleManager.listManager[Random.Range(0, bubbleManager.listManager.Count)];

            // Di chuyển bot đến gần vị trí x của bubble
            Vector3 targetPosition = new Vector3(randomBubble.transform.position.x, transform.position.y, transform.position.z);
            StartCoroutine(MoveBotToPosition(targetPosition));

            // Đợi bot di chuyển đến gần bubble
            yield return new WaitForSeconds(0.5f);

            // Thực hiện bật nhảy để "tấn công" bubble
            JumpAttack(randomBubble);

            // Đợi một chút để tấn công hoàn tất
            yield return new WaitForSeconds(0.5f);

            // Kiểm tra va chạm giữa bot và bubble
            Collider2D bubbleCollider = randomBubble.GetComponent<Collider2D>();
            if (bubbleCollider != null && bubbleCollider.bounds.Intersects(GetComponent<Collider2D>().bounds))
            {
                Debug.Log("Collision detected between bot and bubble");
            }
        }
    }

    // Coroutine di chuyển bot đến vị trí target
    private IEnumerator MoveBotToPosition(Vector3 targetPosition)
    {
        float timeToMove = 0.5f;
        float elapsedTime = 0f;

        Vector3 startPosition = transform.position;
        while (elapsedTime < timeToMove)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / timeToMove));
            elapsedTime += Time.deltaTime;

            // Check if the bot is within the desired range on the X-axis (-0.2 to 0.2 of target position)
            if (Mathf.Abs(transform.position.x - targetPosition.x) <= 0.2f)
            {
                break; // Stop moving if the bot is close enough
            }

            yield return null;
        }

        // Ensure bot is within the range, but not necessarily exactly at the target position
        if (Mathf.Abs(transform.position.x - targetPosition.x) > 0.2f)
        {
            transform.position = targetPosition.x > transform.position.x ?
                                 new Vector3(targetPosition.x - 0.2f, transform.position.y, transform.position.z) :
                                 new Vector3(targetPosition.x + 0.2f, transform.position.y, transform.position.z);
        }
    }

    // Coroutine thực hiện bật nhảy (sử dụng Rigidbody2D)
    private void JumpAttack(BubbleTungnd bubble)
    {
        turnJump++;
        // Calculate the jump height based on the vertical distance to the bubble
        float jumpHeight = Mathf.Abs(bubble.transform.position.y - transform.position.y) * 1.6f; // Increase jump height factor

        // Reset Y velocity and add force to jump
        rb.velocity = new Vector2(rb.velocity.x, 0); // Reset Y velocity to ensure correct jump
        rb.AddForce(Vector2.up * 15, ForceMode2D.Impulse); // Stronger jump
    }

    // Method to check if the bot is grounded (collision with the ground)
    private bool isGrounded()
    {
        // Use a small box/circle collider check to see if the bot is touching the ground
        Vector2 bottomOfBot = new Vector2(transform.position.x, transform.position.y - col.bounds.extents.y); // Bottom of the bot
        RaycastHit2D hit = Physics2D.Raycast(bottomOfBot, Vector2.down, 0.1f, LayerMask.GetMask("Ground")); // Check for Ground layer
        Debug.DrawLine(bottomOfBot, Vector2.down * 0.1f, hit.collider != null ? Color.yellow : Color.red);

        return hit.collider != null; // Return true if we hit something
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isOnGround = true;
            StartCoroutine(Patrol());
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
