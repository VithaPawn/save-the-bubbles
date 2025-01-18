using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Bubble : MonoBehaviour {
    private float lastClickTime = 0f;
    [SerializeField] private float doubleClickThreshold = 0.3f;
    private Rigidbody2D rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
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
        if (!rigid) return;
        rigid.AddForce(forceVector * forceMultiplier, ForceMode2D.Impulse);
    }

    public void DestroyMyself()
    {
        Destroy(gameObject);
    }

    public Vector2 GetVelocity()
    {
        if (!rigid) return Vector2.zero;
        else return rigid.velocity;
    }

    public void SetVelocity(Vector2 velocity)
    {
        if (rigid) rigid.velocity = velocity;
    }
}
