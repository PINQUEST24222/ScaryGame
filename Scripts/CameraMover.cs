using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CameraMover : MonoBehaviour
{
    [Header("Camera Movement")]
    public Transform mainCamera;        // Camera to move
    public Transform settingsTarget;    // Settings wall position + rotation
    public float moveSpeed = 2f;

    [Header("Disable These While Moving (IMPORTANT)")]
    public MonoBehaviour[] disableWhileMoving; 
    // Drag flashlight follow, breathing, look scripts here

    [Header("UI Buttons")]
    public Button playButton;
    public Button quitButton;
    public Button backButton;

    private bool moving = false;
    private Transform moveTarget;

    private Vector3 startPosition;
    private Quaternion startRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        if (mainCamera != null)
        {
            startPosition = mainCamera.position;
            startRotation = mainCamera.rotation;
        }

        if (playButton != null)
            playButton.onClick.AddListener(OnPlay);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuit);

        if (backButton != null)
            backButton.onClick.AddListener(MoveCameraBack);
    }

    void Update()
    {
        if (!moving || mainCamera == null || moveTarget == null)
            return;

        // 🔥 FORCE movement (no more fake lerp)
        mainCamera.position = Vector3.MoveTowards(
            mainCamera.position,
            moveTarget.position,
            moveSpeed * Time.deltaTime
        );

        // Smooth rotation
        mainCamera.rotation = Quaternion.Slerp(
            mainCamera.rotation,
            moveTarget.rotation,
            moveSpeed * Time.deltaTime
        );

        // Stop when close
        if (Vector3.Distance(mainCamera.position, moveTarget.position) < 0.01f)
        {
            mainCamera.position = moveTarget.position;
            mainCamera.rotation = moveTarget.rotation;
            moving = false;
            SetCameraScripts(true);
        }
    }

    // =======================
    // BUTTON CALLS
    // =======================

    public void MoveCameraToSettings()
    {
        moveTarget = settingsTarget;
        moving = true;
        SetCameraScripts(false);
    }

    public void MoveCameraBack()
    {
        GameObject temp = new GameObject("TempCameraTarget");
        temp.transform.position = startPosition;
        temp.transform.rotation = startRotation;

        moveTarget = temp.transform;
        moving = true;
        SetCameraScripts(false);

        Destroy(temp, 3f);
    }

    private void OnPlay()
    {
        SceneManager.LoadScene("niggy]"); // change name if needed
    }

    private void OnQuit()
    {
        Application.Quit();
    }

    // =======================
    // CAMERA CONTROL LOCK
    // =======================

    void SetCameraScripts(bool enabled)
    {
        foreach (MonoBehaviour script in disableWhileMoving)
        {
            if (script != null)
                script.enabled = enabled;
        }
    }
}
