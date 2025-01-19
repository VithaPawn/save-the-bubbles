using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Bubble : MonoBehaviour {
    private float lastClickTime = 0f;
    private float doubleClickThreshold = 0.3f;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    private void OnMouseDown()
    {
        float currentTime = Time.time;

        // Check for double-click
        if (currentTime - lastClickTime <= doubleClickThreshold)
        {
            HandleDoubleClick();
        }

        lastClickTime = currentTime;
    }

    private void HandleDoubleClick()
    {
        BubblesManager.instance.DivideBubble(this);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Bubble bubble))
        {
            BubblesManager.instance.OnBubbleCollide(this, bubble);
        }
    }

    public void MoveByForce(Vector2 forceVector, float forceMultiplier)
    {
        if (!rb) return;
        rb.AddForce(forceVector * forceMultiplier, ForceMode2D.Impulse);
    }

    public void DestroyMyself()
    {
        BubblesManager.instance.RemoveBubble(this);
        Destroy(gameObject);
    }

    public void Freeze(Color freezeColor, float freezeDuration)
    {
        StartCoroutine(FreezeCoroutine(freezeColor, freezeDuration));
    }

    private IEnumerator FreezeCoroutine(Color freezeColor, float freezeDuration)
    {
        // Freeze the bubble
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = freezeColor;
        }

        // Wait for the freeze duration
        yield return new WaitForSeconds(freezeDuration);

        // Restore original state
        rb.isKinematic = false;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    public Vector2 GetVelocity()
    {
        if (!rb) return Vector2.zero;
        else return rb.velocity;
    }

    public void SetVelocity(Vector2 velocity)
    {
        if (rb) rb.velocity = velocity;
    }

}
