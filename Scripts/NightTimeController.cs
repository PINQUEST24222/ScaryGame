using UnityEngine;
using TMPro;

public class NightTimeController : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI timeText;

    [Header("Night Settings")]
    public int startHour = 12;
    public int endHour = 6;
    public float secondsPerHour = 60f; // 1 in-game hour = 60 real seconds

    int currentHour;
    float hourTimer;

    void Start()
    {
        currentHour = startHour;
        UpdateClockUI();
    }

    void Update()
    {
        hourTimer += Time.deltaTime;

        if (hourTimer >= secondsPerHour)
        {
            hourTimer = 0f;
            AdvanceHour();
        }
    }

    void AdvanceHour()
    {
        currentHour++;

        if (currentHour > 12)
            currentHour = 1;

        UpdateClockUI();

        if (currentHour == endHour)
        {
            EndNight();
        }
    }

    void UpdateClockUI()
    {
        timeText.text = currentHour + ":00";
    }

    void EndNight()
    {
        Debug.Log("Night Survived");
        // sunrise, stop monster, win screen, etc
    }
}
