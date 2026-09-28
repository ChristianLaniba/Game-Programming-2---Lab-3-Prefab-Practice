using UnityEngine;

public class ShiftLockCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;            // Drag your Player here

    [Header("Offset")]
    public Vector3 shoulderOffset = new Vector3(0.5f, 1.5f, 0f); // Right, Up, Forward

    [Header("Zoom")]
    public float distance = 6f;         // Current distance
    public float minDistance = 2f;      // Closest zoom
    public float maxDistance = 12f;     // Farthest zoom
    public float zoomSpeed = 4f;        // How fast scroll changes distance
    public float zoomSmooth = 10f;      // How smoothly the distance lerps

    [Header("Rotation")]
    public float mouseSensitivity = 3f;
    public float minPitch = -40f;
    public float maxPitch = 70f;
    public bool lockCursor = true;

    [Header("Smoothing")]
    public float positionSmooth = 12f;

    private float yaw;
    private float pitch = 15f;
    private float targetDistance;       // What we're zooming toward

    void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (target != null)
            yaw = target.eulerAngles.y;

        targetDistance = distance;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Mouse look
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Scroll zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.0001f)
        {
            targetDistance -= scroll * zoomSpeed;
            targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
        }

        // Smoothly move current distance toward target
        distance = Mathf.Lerp(distance, targetDistance, zoomSmooth * Time.deltaTime);

        // Build rotation
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        // Offset from the player
        Vector3 pivot = target.position + Vector3.up * shoulderOffset.y;
        Vector3 desiredPos = pivot + rotation * new Vector3(shoulderOffset.x, 0f, -distance);

        // Smooth position
        transform.position = Vector3.Lerp(transform.position, desiredPos, positionSmooth * Time.deltaTime);

        // Look at pivot
        transform.LookAt(pivot);
    }
}