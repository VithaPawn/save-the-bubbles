using UnityEngine;

public class WindController : MonoBehaviour {
    [SerializeField] private GameObject bubble; // Assign your soap ball's Rigidbody in the Inspector
    [SerializeField] public float windForceMultiplier = 3f; // Adjust to control the wind force

    private Vector3 dragStartPos;
    private Vector3 dragEndPos;
    private Vector3 dragVector;
    private bool isDragging;
    private GameObject movementAllowedArea;

    private void Awake()
    {
        dragVector = Vector3.zero;
        movementAllowedArea = GameObject.FindGameObjectWithTag(GameConstants.PLAYING_AREA_TAG);
        bubble = GameObject.FindGameObjectWithTag(GameConstants.PLAYER_TAG);
    }

    void Update()
    {
        HandleMouseInput();
    }

    private void LateUpdate()
    {
        if (dragVector != Vector3.zero)
        {
            LimitMovementSpace();
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Record the starting position of the drag
            dragStartPos = Input.mousePosition;
            isDragging = true;
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            // Record the ending position of the drag and calculate the direction
            dragEndPos = Input.mousePosition;
            dragVector = (dragEndPos - dragStartPos).normalized;
            dragVector = new Vector3(dragVector.x, dragVector.y, 0);

            // Apply wind force
            ApplyWindForce(dragVector);
            isDragging = false;
        }
    }

    private void ApplyWindForce(Vector3 dragVector)
    {
        // Convert screen space drag vector to world space
        Vector3 worldDirection = new Vector3(dragVector.x, 0, dragVector.y);

        // Apply force to the soap ball
        if (bubble.TryGetComponent(out Rigidbody2D bubbleRigid))
        {
            bubbleRigid.AddForce(worldDirection * windForceMultiplier, ForceMode2D.Impulse);
        }
    }

    private void LimitMovementSpace()
    {
        //Get dashing target width
        Renderer bubbleRenderer = bubble.GetComponent<Renderer>();
        float bubbleWidth = bubbleRenderer ? bubbleRenderer.bounds.size.x : 0;

        bubble.transform.position = MovementUtilities.LimitPositionInsideArea(movementAllowedArea, bubble, bubble.transform.position);
    }

    private float GetObjectWidth(GameObject obj)
    {
        Renderer objRenderer = obj.GetComponent<Renderer>();
        return objRenderer ? objRenderer.bounds.size.x : 0;
    }
}
