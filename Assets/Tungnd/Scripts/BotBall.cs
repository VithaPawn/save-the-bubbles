using UnityEngine;

public class BotBall : MonoBehaviour {
    [Header("Freeze Status")]
    [SerializeField] private Color freezeColor = Color.blue;
    [SerializeField] private float freezeDuration = 2f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, 5f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if (collision.gameObject.CompareTag("Bot"))
        //{
        //    Destroy(gameObject);
        //}
        if (collision.gameObject.TryGetComponent(out Bubble bubble))
        {
            bubble.Freeze(freezeColor, freezeDuration);
            Destroy(gameObject);
        }
    }
}
