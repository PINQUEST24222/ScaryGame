using UnityEngine;

public class SanityManager : MonoBehaviour
{
    [Header("Sanity")]
    public static float sanity = 100f;
    public float drainRate = 3f;

    [Header("Breathing Audio")]
    public AudioSource breathingSource;
    public AnimationCurve volumeCurve;

    public static bool isDead = false;

    void Start()
    {
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

        if (sanity <= 0f)
        {
            AudioListener.volume = 0f;
            Invoke("KillPlayer", 2f);
        }
    }

    public static void KillPlayer()
    {
        Debug.Log("YOU DIED!!!");
        isDead = true;
    }
}
