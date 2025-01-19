using UnityEngine;

public class ShootBallFrozenAttack : MonoBehaviour, IAttackBehavior {
    [SerializeField] private GameObject ballFrozenPrefab; // Prefab quả bóng băng
    [SerializeField] private float shootForce = 13f; // Lực bắn của bóng
    public bool IsManualSetAngle = false;
    public int Angle = 0;
    public void ExecuteAttack(BotController bot)
    {
        if (ballFrozenPrefab == null)
        {
            Debug.LogWarning("Ball Frozen Prefab chưa được gán!");
            return;
        }

        // Tạo prefab tại vị trí của bot
        GameObject ball = Instantiate(ballFrozenPrefab, bot.transform.position, Quaternion.identity);

        // Lấy Rigidbody từ prefab
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogWarning("Prefab không có Rigidbody2D!");
            return;
        }

        float angle = 0;
        // Tính toán góc bắn (-45 hoặc 45 độ)
        if (IsManualSetAngle)
        {
            angle = Random.Range(0, 2) == 0 ? 135f : 45f; // Random giữa -45 và 45
        }
        else
        {
            angle = Angle;
        }

        Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

        // Áp dụng lực bắn
        rb.AddForce(direction * shootForce, ForceMode2D.Impulse);
    }
}
