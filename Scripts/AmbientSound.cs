using UnityEngine;

public class AmbientSound : MonoBehaviour
{
    public AudioClip clip;
    private AudioSource source;
    public float volume;
    public float minTime;
    public float maxTime;
    private float nextTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nextTime = Time.time + Random.Range(minTime, maxTime);
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time >= nextTime)
        {
            nextTime = Time.time + Random.Range(minTime, maxTime);
            source.PlayOneShot(clip, volume);
        }
    }
}
