using UnityEngine;

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

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 rot = transform.eulerAngles;
        targetX = rot.y;
        targetY = rot.x;
        currentX = targetX;
        currentY = targetY;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * 100f * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * 100f * Time.deltaTime;

        targetX += mouseX;
        targetY -= mouseY;
        targetY = Mathf.Clamp(targetY, minY, maxY);

        currentX = Mathf.SmoothDamp(currentX, targetX, ref xVel, smoothTime);
        currentY = Mathf.SmoothDamp(currentY, targetY, ref yVel, smoothTime);

        transform.rotation = Quaternion.Euler(currentY, currentX, 0f);
    }
}
