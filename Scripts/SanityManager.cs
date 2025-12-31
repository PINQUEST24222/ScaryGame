using UnityEngine;

public class SanityManager : MonoBehaviour
{
    [Header("Sanity")]
    public float sanity = 100f;
    public float drainRate = 5f;
    public float recoverRate = 3f;

    [Header("Flashlight")]
    public FlashlightController flashlight; // ✅ correct script

    [Header("Breathing Audio")]
    public AudioSource breathingSource;
    public AnimationCurve volumeCurve;

    bool isDead = false;

    void Start()
    {
        breathingSource.loop = true;
        breathingSource.volume = 0f;
        breathingSource.Play();
    }

    void Update()
    {
        if (isDead) return;

        if (!flashlight.IsOn)
        {
            sanity -= drainRate * Time.deltaTime;
            Debug.Log(sanity);
        }
        else
        {
            sanity += recoverRate * Time.deltaTime;
        }

        sanity = Mathf.Clamp(sanity, 0f, 100f);

        UpdateBreathing();

        if (sanity <= 0f)
        {
            SanityDeath();
        }
    }

    void UpdateBreathing()
    {
        breathingSource.volume = volumeCurve.Evaluate(sanity);
    }

    void SanityDeath()
    {
        isDead = true;

        // absolute silence
        AudioListener.volume = 0f;

        Debug.Log("Sanity hit 0 — everything goes silent");
        // delay monster attack here 👁️
    }
}
