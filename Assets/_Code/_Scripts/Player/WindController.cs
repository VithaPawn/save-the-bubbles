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

    //[Header("Visual")]
    //[SerializeField] private DashingTarget dashingTarget;
    //[SerializeField] private DrawingCircle drawingCircle;
    //[Header("Dashing Attributes")]
    //[SerializeField] private float dashingTargetMovementSpeed;
    //[SerializeField] private float dashingDuration;
    //[Header("Event Channels")]
    //[SerializeField] private VoidEventChannelSO startDashingSO;
    //[SerializeField] private VoidEventChannelSO endDashingSO;

    //private GameObject movementAllowedArea;
    //private CustomFloatingJoystic floatingJoystick;

    //private void Awake()
    //{
    //    movementAllowedArea = GameObject.FindGameObjectWithTag(GameConstants.PLAYING_AREA_TAG);
    //    GameObject joystick = GameObject.FindGameObjectWithTag(GameConstants.JOYSTICK_TAG);
    //    floatingJoystick = joystick.GetComponent<CustomFloatingJoystic>();
    //}

    //private void OnEnable()
    //{
    //    floatingJoystick.PointerUpEvent += OnPointerUp;
    //    floatingJoystick.PointerDownEvent += OnPointerDown;
    //}

    //private void OnDisable()
    //{
    //    floatingJoystick.PointerUpEvent -= OnPointerUp;
    //    floatingJoystick.PointerDownEvent -= OnPointerDown;
    //}

    //private void OnPointerUp()
    //{
    //    transform.DOMove(dashingTarget.GetPosition(), dashingDuration);
    //    StartCoroutine(PlayDashEffect());
    //}

    //private void OnPointerDown()
    //{
    //    dashingTarget.Show();
    //}

    //private void Update()
    //{
    //    if (floatingJoystick.Direction != Vector2.zero)
    //    {
    //        HandleMovement();
    //    }
    //}

    //private void HandleMovement()
    //{
    //    Vector2 joystickDirectionVector = floatingJoystick.Direction.normalized;
    //    Vector3 movementVector = new Vector3(joystickDirectionVector.x, joystickDirectionVector.y, 0);
    //    MoveDashingTarget(movementVector);
    //}

    //private IEnumerator PlayDashEffect()
    //{
    //    startDashingSO.RaiseEvent();

    //    //Wait for dash animation
    //    yield return new WaitForSeconds(dashingDuration);

    //    endDashingSO.RaiseEvent();
    //}

    //private void MoveDashingTarget(Vector3 movementVector)
    //{
    //    Vector3 dashingTargetPos = dashingTarget.GetPosition();
    //    Vector3 tempDashingTargetPos = dashingTargetPos + movementVector * dashingTargetMovementSpeed * Time.deltaTime;

    //    //Get dashing target width
    //    Renderer objRenderer = dashingTarget.gameObject.GetComponent<Renderer>();
    //    float dashingTargetWidth = objRenderer ? objRenderer.bounds.size.x : 0;

    //    float distance = Vector3.Distance(tempDashingTargetPos, transform.position) + dashingTargetWidth / 2;
    //    dashingTarget.SetPosition(MovementUtilities.LimitPositionInsideArea(movementAllowedArea, dashingTarget.gameObject,
    //        distance <= drawingCircle.GetRadius() ? tempDashingTargetPos : dashingTargetPos));
    //}


    //private float GetObjectWidth(GameObject obj)
    //{
    //    Renderer objRenderer = obj.GetComponent<Renderer>();
    //    return objRenderer ? objRenderer.bounds.size.x : 0;
    //}

    //public DashingTarget GetDashingTarget() { return dashingTarget; }
}
