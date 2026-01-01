using UnityEngine;

public class TitleScreenFlashlight : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;
    public Light flashlight;

    [Header("Flashlight Settings")]
    public float rotationSpeed = 10f;
    public float distanceAhead = 10f;

    [Header("Flicker Settings")]
    public float flickerAmount = 0.1f;
    public float flickerSpeed = 20f;

    [Header("Camera Breathing")]
    public float breathAmplitude = 0.05f;
    public float breathSpeed = 1.5f;

    private float baseIntensity;
    private float breathTimer;

    void Start()
    {
        if (flashlight != null)
            baseIntensity = flashlight.intensity;
    }

    void Update()
    {
        if (mainCamera == null || flashlight == null) return;

        // 🔦 Flashlight follow cursor
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = distanceAhead;
        Vector3 targetPos = mainCamera.ScreenToWorldPoint(mousePos);
        Vector3 direction = (targetPos - flashlight.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        flashlight.transform.rotation =
            Quaternion.Slerp(flashlight.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // ✨ Flicker
        float flicker = (Mathf.PerlinNoise(Time.time * flickerSpeed, 0f) - 0.5f) * 2f * flickerAmount;
        flashlight.intensity = baseIntensity + flicker;

        // 🫁 Camera breathing (ADDITIVE, NOT LOCKING)
        breathTimer += Time.deltaTime * breathSpeed;
        float breathOffset = Mathf.Sin(breathTimer) * breathAmplitude;

        mainCamera.transform.position += mainCamera.transform.up * breathOffset * Time.deltaTime;
    }
}
