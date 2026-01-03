using UnityEngine;
using UnityEngine.Audio;

public class MonsterController : MonoBehaviour
{
    [Header("References")]
    public FlashlightController flashlight;
    public Transform door;          // door to open

    [Header("Door Settings")]
    public float minWaitTime;
    public float maxWaitTime;
    public float doorOpenSpeed; // degrees per second
    public float doorCloseSpeed;
    public float maxOpenDegrees;

    [Header("Door Slam Settings")]
    public AudioClip doorBangSFX;
    public float doorBangVolume;    

    [Header("Audio")]
    public AudioClip[] stageSounds;  // one-shot sounds per stage (door creaks, thumps, etc)
    private AudioSource audioSource;
    private AudioLowPassFilter lowPass;
    public float maxMonsterVolume;

    private float doorOpenAmount;
    private float nextOpenTime;

    void Start()
    {
        nextOpenTime = -1;
        audioSource = GetComponent<AudioSource>();
        lowPass = GetComponent<AudioLowPassFilter>();
    }

    void Update()
    {
        if (SanityManager.isDead || SanityManager.sanity <= 0f) return;

        if (Time.time >= nextOpenTime)
        {
            if(!audioSource.isPlaying)
                audioSource.Play();

            bool open = doorOpenAmount > 0f;
            lowPass.enabled = true;
            doorOpenAmount += (flashlight.IsOn && door.gameObject.GetComponent<MeshRenderer>().isVisible) ? -doorCloseSpeed * Time.deltaTime : doorOpenSpeed * Time.deltaTime;
            doorOpenAmount = Mathf.Clamp(doorOpenAmount, 0, maxOpenDegrees);
                      
            door.localRotation = Quaternion.Euler(0f, doorOpenAmount, 0f);
            lowPass.cutoffFrequency = Mathf.Lerp(1000, 5000, doorOpenAmount/maxOpenDegrees);
            audioSource.volume = Mathf.Lerp(0, maxMonsterVolume, doorOpenAmount/maxOpenDegrees);

            if (open && doorOpenAmount <= 0)
            {
                audioSource.Stop();
                Debug.Log("BANG!");
                open = false;
                lowPass.enabled = false;
                audioSource.volume = 1f;
                audioSource.PlayOneShot(doorBangSFX, doorBangVolume);
                nextOpenTime = Time.time + Random.Range(minWaitTime, maxWaitTime);
            }

            if (doorOpenAmount >= maxOpenDegrees)
            {
                audioSource.volume = 0;
                SanityManager.sanity = 0;
            }
        }
    }
}
