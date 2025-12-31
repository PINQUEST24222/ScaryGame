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
                stageTimer += Time.deltaTime;

                if (stageTimer >= timePerStage)
                {
                    stageTimer = 0f;
                    AdvanceStage();
                }

                HandleDoorOpening();
            }
            else
            {
                ResetMonster();
            }
        }
        else
        {
            HandleInsideMovement();
        }
    }

    void AdvanceStage()
    {
        currentStage++;
        Debug.Log("Monster Stage: " + currentStage);

        // Play stage sound if assigned
        if (stageSounds != null && stageSounds.Length > currentStage && stageSounds[currentStage] != null)
        {
            monsterAudio.PlayOneShot(stageSounds[currentStage]);
        }

        // Increase ambient volume with stage
        if (monsterAudio != null)
        {
            monsterAudio.volume = Mathf.Clamp01(currentStage * 0.25f);
            if (!monsterAudio.isPlaying)
                monsterAudio.Play();
        }

        if (currentStage >= maxStage)
        {
            KillPlayer();
        }
    }

    void ResetMonster()
    {
        if (currentStage > 0)
        {
            currentStage = 0;
            stageTimer = 0f;

            doorOpenAmount = 0f;
            if (door != null)
                door.localRotation = Quaternion.identity;

            if (monsterAudio != null)
            {
                monsterAudio.Stop();
                monsterAudio.volume = 0f;
            }

            insideRoom = false;
            doorSlammed = false;

            Debug.Log("Monster reset by flashlight");
        }
    }

    void HandleDoorOpening()
    {
        if (door == null) return;

        // If flashlight is on while door is opening → slam
        if (flashlight.IsOn && doorOpenAmount > 0f && !doorSlammed)
        {
            doorSlammed = true;

            // instantly close door
            doorOpenAmount = 0f;
            door.localRotation = Quaternion.identity;

            // play bang
            if (audioSource != null && doorBangSFX != null)
                audioSource.PlayOneShot(doorBangSFX, 100f);

            Debug.Log("Door slammed by monster!");
            return;
        }

        // normal slow opening
        if (!doorSlammed)
        {
            doorOpenAmount += Time.deltaTime * 0.1f; // slow open
            doorOpenAmount = Mathf.Clamp01(doorOpenAmount);
            door.localRotation = Quaternion.Euler(0f, doorOpenAmount * 90f, 0f);

            if (doorOpenAmount >= 1f)
            {
                EnterRoom();
            }
        }
    }

    void EnterRoom()
    {
        insideRoom = true;
        distanceFromPlayer = maxDistance;
        Debug.Log("Monster entered the room (invisible)!");
    }

    void HandleInsideMovement()
    {
        // Only logical distance; no 3D monster
        float dir = flashlight.IsOn ? -1f : 1f;
        distanceFromPlayer += dir * insideMoveSpeed * Time.deltaTime;
        distanceFromPlayer = Mathf.Clamp(distanceFromPlayer, minDistance, maxDistance);

        // Scale audio intensity with closeness
        if (monsterAudio != null)
        {
            monsterAudio.volume = Mathf.Lerp(1f, 0f, distanceFromPlayer / maxDistance);
        }

        // Increase sanity drain based on proximity
        sanityManager.sanity -= (1f - distanceFromPlayer / maxDistance) * Time.deltaTime * 2f;

        if (distanceFromPlayer <= killDistance)
        {
            KillPlayer();
        }
    }

    void KillPlayer()
    {
        Debug.Log("Monster killed player!");
        isDead = true;
        // trigger jumpscare, blackout, restart, etc
    }
}
