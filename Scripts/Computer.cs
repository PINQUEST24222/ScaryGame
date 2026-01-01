using UnityEngine;

public class Computer : MonoBehaviour
{
    public GameObject screen;
    public float minTime;
    public float maxTime;
    private float nextTurnOnTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        screen.SetActive(false);
        nextTurnOnTime = Time.time + Random.Range(minTime, maxTime);
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time >= nextTurnOnTime)
        {
            screen.SetActive(true);
        }

        if (!screen.activeSelf)
        {
            nextTurnOnTime = Time.time + Random.Range(minTime, maxTime);
        }
    }
}
