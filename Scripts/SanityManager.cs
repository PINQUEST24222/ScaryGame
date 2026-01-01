using UnityEngine;

public class SanityManager : MonoBehaviour
{
    [Header("Sanity")]
    public static float sanity = 100f;
    public float drainRate = 3f;

    [Header("Breathing Audio")]
    public AudioSource breathingSource;
    public AnimationCurve volumeCurve;

    [Header("Jumpscare Audio")]
    public GameObject jumpscareObject;
    public AudioClip jumpscareClip;
    public float jumpscareVolume;

    public static bool isDead = false;

    void Start()
    {
        jumpscareObject.SetActive(false);
        breathingSource.loop = true;
        breathingSource.volume = 0f;
        breathingSource.Play();
    }

    void Update()
    {
        if (isDead) return;

        sanity -= drainRate * Time.deltaTime;
        sanity = Mathf.Clamp(sanity, 0f, 100f);

        breathingSource.volume = volumeCurve.Evaluate(sanity);

        if (sanity <= 0f && !isDead)
        {
            isDead = true;
            breathingSource.Stop();
            Invoke("KillPlayer", 2f);
        }
    }

    void KillPlayer()
    {
        Debug.Log("YOU DIED!!!");
        breathingSource.volume = 1f;
        breathingSource.PlayOneShot(jumpscareClip, jumpscareVolume);
        jumpscareObject.SetActive(true);
    }
}
