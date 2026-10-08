using UnityEngine;

public class CameraBob : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController playerController;

    [Header("Standing / Breathing")]
    [SerializeField] private float idleBobSpeed = 1.5f;
    [SerializeField] private float idleBobAmount = 0.008f;
    [SerializeField] private float idleSwayAmount = 0.005f;

    [Header("Walking")]
    [SerializeField] private float walkBobSpeed = 8f;
    [SerializeField] private float walkBobAmount = 0.035f;
    [SerializeField] private float walkSwayAmount = 0.02f;

    [Header("Sprinting")]
    [SerializeField] private float sprintBobSpeed = 11f;
    [SerializeField] private float sprintBobAmount = 0.055f;
    [SerializeField] private float sprintSwayAmount = 0.03f;

    [Header("Smoothness")]
    [SerializeField] private float smoothSpeed = 8f;

    private Vector3 startLocalPosition;
    private float bobTimer;

    private void Start()
    {
        startLocalPosition = transform.localPosition;
    }

   private void Update()
{
    if (playerController == null)
        return;

    float horizontal = Input.GetAxisRaw("Horizontal");
    float vertical = Input.GetAxisRaw("Vertical");

    bool isMoving = Mathf.Abs(horizontal) > 0.01f ||
                    Mathf.Abs(vertical) > 0.01f;

    bool isSprinting = Input.GetKey(KeyCode.LeftShift) && isMoving;

    float bobSpeed;
    float bobAmount;
    float swayAmount;

    if (isSprinting)
    {
        bobSpeed = sprintBobSpeed;
        bobAmount = sprintBobAmount;
        swayAmount = sprintSwayAmount;
    }
    else if (isMoving)
    {
        bobSpeed = walkBobSpeed;
        bobAmount = walkBobAmount;
        swayAmount = walkSwayAmount;
    }
    else
    {
        bobSpeed = idleBobSpeed;
        bobAmount = idleBobAmount;
        swayAmount = idleSwayAmount;
    }

    bobTimer += Time.deltaTime * bobSpeed;

    float verticalBob = Mathf.Sin(bobTimer) * bobAmount;
    float horizontalSway = Mathf.Cos(bobTimer * 0.5f) * swayAmount;

    Vector3 targetPosition = startLocalPosition;

    targetPosition.y += verticalBob;
    targetPosition.x += horizontalSway;

    transform.localPosition = Vector3.Lerp(
        transform.localPosition,
        targetPosition,
        smoothSpeed * Time.deltaTime
    );
}
}