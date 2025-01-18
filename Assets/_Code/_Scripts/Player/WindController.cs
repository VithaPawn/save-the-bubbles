using System.Collections.Generic;
using UnityEngine;

public class WindController : MonoBehaviour {
    [SerializeField] private List<GameObject> bubbles;
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
            Vector2 normalizedVector = (dragEndPos - dragStartPos).normalized;

            // Calculate ratio and adjust drag vector
            Vector3 dragVector = new Vector3(normalizedVector.x, normalizedVector.y, 0);

            foreach (var bubble in bubbles)
            {
                if (IsWithinEffectRadius(bubble.transform.position))
                {

                    ApplyWindForce(dragVector, bubble);
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

    private void ApplyWindForce(Vector2 vector, GameObject obj)
    {
        Rigidbody2D bubbleRigid = obj.GetComponent<Rigidbody2D>();
        if (bubbleRigid == null) return;
        bubbleRigid.AddForce(vector * windForceMultiplier, ForceMode2D.Impulse);
    }
    //private Vector2 AdjustVectorWithRatio(Vector2 dragVector, GameObject obj)
    //{
    //    // Calculate screen bounds in world space
    //    float screenHeight = Camera.main.orthographicSize * 2f;
    //    float screenWidth = screenHeight * Camera.main.aspect;

    //    // Calculate the object's current position
    //    Vector2 objectPosition = obj.transform.position;

    //    // Determine distance to left or right border based on drag direction
    //    float distanceToBorder = dragVector.x > 0
    //        ? objectPosition.x + (screenWidth / 2f) // Distance to left border
    //        : (screenWidth / 2f) - objectPosition.x;  // Distance to right border

    //    // Calculate the ratio of screen height to the distance
    //    float ratio = screenWidth / distanceToBorder;

    //    // Scale the drag vector using the ratio
    //    return dragVector * ratio;
    //}
}
