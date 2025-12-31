using UnityEngine;

public class MonsterController : MonoBehaviour
{
    [Header("References")]
    public FlashlightController flashlight;
    public SanityManager sanityManager;
    public Transform door;          // door to open

    [Header("Stage Settings")]
    public float timePerStage = 6f;
    public int maxStage = 4;

    [Header("Door Settings")]
    public float doorOpenSpeed = 20f; // degrees per second

    [Header("Door Slam Settings")]
    public AudioClip doorBangSFX;
    public AudioSource audioSource; // for playing one-shot sounds
    public float slamSpeed = 10f; // how fast the door closes
    private bool doorSlammed;

    [Header("Inside ")]
    public float insideMoveSpeed = 1.5f;
    public float minDistance = 0.4f;
    public float maxDistance = 5f;
    public float killDistance = 0.7f;

    [Header("Audio")]
    public AudioSource monsterAudio; // looping breathing / ambient
    public AudioClip[] stageSounds;  // one-shot sounds per stage (door creaks, thumps, etc)

    private float stageTimer;
    private int currentStage;
    private bool isDead;

    private float doorOpenAmount; // 0 = closed, 1 = fully open
    private float distanceFromPlayer;
    private bool insideRoom;

    void Start()
    {
        currentStage = 0;
        stageTimer = 0f;
        insideRoom = false;
        distanceFromPlayer = maxDistance;

        if (monsterAudio != null)
        {
            monsterAudio.Stop();
            monsterAudio.volume = 0f;
            monsterAudio.loop = true;
        }
    }

    void Update()
    {
        if (isDead) return;

        // Sanity death shortcut
        if (sanityManager.sanity <= 0f)
        {
            Invoke(nameof(KillPlayer), 2f);
            isDead = true;
            return;
        }

        if (!insideRoom)
        {
            if (!flashlight.IsOn)
            {
                doorOpenAmount += Time.deltaTime * 0.1f; // slow open
                doorOpenAmount = Mathf.Clamp01(doorOpenAmount);
                door.localRotation = Quaternion.Euler(0f, doorOpenAmount * 90f, 0f);
            }
            else
            {
                if(doorOpenAmount > 0)
                {
                    // Slam door if it was open
                    audioSource.PlayOneShot(doorBangSFX, 100f);
                }
                doorOpenAmount = 0f;
                door.localRotation = Quaternion.identity;
            }
        }
        else
        {
            HandleInsideMovement();
        }
    }

    void KillPlayer()
    {
        Debug.Log("Monster killed player!");
        isDead = true;
        // trigger jumpscare, blackout, restart, etc
    }
}
