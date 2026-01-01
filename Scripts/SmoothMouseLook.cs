using UnityEngine;
using UnityEngine.InputSystem;

public class SmoothMouseLook : MonoBehaviour
{
    [Header("Sensitivity")]
    public float mouseSensitivity = 2.2f;

    [Header("Smooth Delay (Higher = More Lag)")]
    public float smoothTime = 0.1f;

    [Header("Vertical Clamp")]
    public float minY = -70f;
    public float maxY = 70f;

    float targetX;
    float targetY;

    float currentX;
    float currentY;

    float xVel;
    float yVel;

    private InputAction lookAction;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 rot = transform.eulerAngles;
        targetX = rot.y;
        targetY = rot.x;
        currentX = targetX;
        currentY = targetY;

        lookAction = InputSystem.actions.FindAction("Look");
    }

    void Update()
    {
        Vector2 lookDir = lookAction.ReadValue<Vector2>() * mouseSensitivity * Time.deltaTime;

        targetX += lookDir.x;
        targetY -= lookDir.y;
        targetY = Mathf.Clamp(targetY, minY, maxY);

        currentX = Mathf.SmoothDamp(currentX, targetX, ref xVel, smoothTime);
        currentY = Mathf.SmoothDamp(currentY, targetY, ref yVel, smoothTime);

        transform.rotation = Quaternion.Euler(currentY, currentX, 0f);
    }
}
