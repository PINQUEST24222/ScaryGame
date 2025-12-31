using UnityEngine;
using UnityEngine.InputSystem;

public class FlashlightController : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] AudioClip _onSFX;
    [SerializeField] AudioClip _offSFX;
    [SerializeField] AudioClip _deadSFX;
    private InputAction flashlightAction;

    [Header("Battery")]
    public float Battery = 100f;
    public float DrainRate = 12f;
    public float RechargeRate = 6f;

    [Header("Low Battery Flicker")]
    [SerializeField] float _flickerThreshold = 20f;
    [SerializeField] float _minFlickerInterval = 0.05f;
    [SerializeField] float _maxFlickerInterval = 0.25f;

    private float _flickerTimer;
    private bool _isFlickering;

    private GameObject _cameraObject;
    private GameObject _lightSource;
    private AudioSource _audioSource;
    private Vector3 _offset;

    public bool IsOn { get; private set; }
    private readonly float _speed = 5f;
    public bool IsEnabled = true;

    private void Awake()
    {
        _cameraObject = Camera.main.gameObject;
        _lightSource = transform.GetChild(0).gameObject;
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _lightSource.SetActive(false);
        _offset = transform.position - _cameraObject.transform.position;
        flashlightAction = InputSystem.actions.FindAction("Flashlight");
    }

    private void Update()
    {
        // Follow camera
        transform.position = _cameraObject.transform.position + _offset;
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            _cameraObject.transform.rotation,
            _speed * Time.deltaTime
        );

        HandleBattery();
        HandleFlicker();

        if (!IsEnabled)
        {
            ForceOff();
            return;
        }

        if (flashlightAction.WasPressedThisFrame())
        {
            if (Battery <= 0f)
            {
                _audioSource.PlayOneShot(_deadSFX);
                return;
            }

            _audioSource.PlayOneShot(_onSFX);
        }

        if (flashlightAction.WasReleasedThisFrame())
        {
            if (Battery <= 0f)
                return;

            _audioSource.PlayOneShot(_offSFX);
            ToggleFlashlight();
        }
    }

    void HandleBattery()
    {
        if (IsOn)
        {
            Battery -= DrainRate * Time.deltaTime;

            if (Battery <= 0f)
            {
                Battery = 0f;
                ForceOff();
                _audioSource.PlayOneShot(_deadSFX);
            }
        }
        else
        {
            Battery += RechargeRate * Time.deltaTime;
        }

        Battery = Mathf.Clamp(Battery, 0f, 100f);
    }

    void ToggleFlashlight()
    {
        IsOn = !IsOn;
        _lightSource.SetActive(IsOn);
    }

    void ForceOff()
    {
        IsOn = false;
        _lightSource.SetActive(false);
    }

    public void PlayFlashlightOffSFX()
    {
        _audioSource.PlayOneShot(_onSFX);
        _audioSource.PlayDelayed(2f);
        _audioSource.PlayOneShot(_offSFX);
    }
    void HandleFlicker()
    {
        if (!IsOn || Battery > _flickerThreshold)
            return;

        _flickerTimer -= Time.deltaTime;

        if (_flickerTimer <= 0f)
        {
            // toggle light briefly
            _lightSource.SetActive(!_lightSource.activeSelf);

            // flicker gets faster as battery gets lower
            float battery01 = Battery / _flickerThreshold;
            float flickerSpeed = Mathf.Lerp(_minFlickerInterval, _maxFlickerInterval, battery01);

            _flickerTimer = Random.Range(flickerSpeed * 0.5f, flickerSpeed);
        }
    }
}
