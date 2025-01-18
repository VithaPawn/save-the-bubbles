using UnityEngine;

public class WindController : MonoBehaviour {
    [SerializeField] private float windForceMultiplier;
    [SerializeField] private float effectRadius;

    private Vector3 dragStartPos;
    private Vector3 dragEndPos;
    private bool isDragging;

    void Update()
    {
        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Record the starting position of the drag
            dragStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dragStartPos.z = 0;
            isDragging = true;
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            // Record the ending position of the drag and calculate the direction
            dragEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dragEndPos.z = 0;
            if (Vector3.Distance(dragStartPos, dragEndPos) < 0.5f) return;
            Vector2 normalizedVector = (dragEndPos - dragStartPos).normalized;

            // Calculate ratio and adjust drag vector
            Vector3 dragVector = new Vector3(normalizedVector.x, normalizedVector.y, 0);

            foreach (var bubble in BubblesManager.instance.GetBubbleList())
            {
                if (IsWithinEffectRadius(bubble.transform.position))
                {

                    bubble.MoveByForce(dragVector, windForceMultiplier);
                }
            }
            isDragging = false;
        }
    }
    private bool IsWithinEffectRadius(Vector3 bubblePosition)
    {
        // Check if the bubble is within the effect radius
        float distance = Vector3.Distance(bubblePosition, dragStartPos);
        return distance <= effectRadius;
    }
}
