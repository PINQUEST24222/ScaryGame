using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GrabStuff : MonoBehaviour
{
    [Header("Grab")]
    public float grabRange;
    public float grabRadius;
    public LayerMask grabbable;
    public float grabDuration;
    public Transform cam;

    [Header("UI")]
    public Image grabProgressBar;

    [Header("Items")]
    public float pillRestoreAmount;

    private InputAction grabAction;
    private float grabProgress;

    void Start()
    {
        grabAction = InputSystem.actions.FindAction("Interact");
        grabProgress = 0;
    }

    void Update()
    {
        grabProgressBar.fillAmount = grabProgress;
        RaycastHit hit;
        if (grabAction.IsPressed() && Physics.SphereCast(cam.transform.position, grabRadius, cam.transform.forward, out hit, grabRange, grabbable, QueryTriggerInteraction.Ignore))
        {
            grabProgress += Time.deltaTime / grabDuration;         
            if(grabProgress >= 1)
            {
                if (hit.transform.gameObject.CompareTag("Pill"))
                {
                    Debug.Log("Took a pill!");
                    grabProgress = 0;
                    SanityManager.sanity += pillRestoreAmount;
                }
                if (hit.transform.gameObject.CompareTag("Computer"))
                {
                    Debug.Log("Turning off computer.");
                    hit.transform.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            grabProgress = 0;
        }
    }
}
