using UnityEngine;

[RequireComponent(typeof(Transform))]
public class Shakeable : MonoBehaviour
{
    public enum ShakeMode
    {
        TRANSLATION_SHAKE,
        ANGULAR_SHAKE,
        FULL_SHAKE
    }

    public static Shakeable CamInstance { get; private set; }

    [SerializeField] private ShakeMode shakeMode = ShakeMode.FULL_SHAKE;

    [SerializeField] private Vector3 maxTranslationShake = Vector3.one * 0.5f;
    [SerializeField] private Vector3 maxAngularShake = Vector3.one * 0.5f;
    [SerializeField, Range(0.1f, 100f)] private float frequency = 25f;
    [SerializeField, Range(0.1f, 10f)] private float recoverySpeed = 1.25f;
    [SerializeField, Range(0.1f, 10f)] private float traumaExponent = 2f;

    private float seed, trauma = 0f;
    private Vector3 originalPosition;

    private void Awake()
    {
        CameraController camController;
        if (transform.TryGetComponent(out camController))
        {
            if (CamInstance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                CamInstance = this;
            }
        }
    }

    private void Start()
    {
        seed = Random.value;
        originalPosition = transform.localPosition;
    }

    public void Setup(ShakeMode shakeMode, Vector3 maxTranslationShake, Vector3 maxAngularShake, float frequency, float recoverySpeed, float traumaExponent)
    {
        this.shakeMode = shakeMode;
        this.maxTranslationShake = maxTranslationShake;
        this.maxAngularShake = maxAngularShake;
        this.frequency = frequency;
        this.recoverySpeed = recoverySpeed;
        this.traumaExponent = traumaExponent;
    }

    private void LateUpdate()
    {
        Shake();
    }

    private void Shake()
    {
        if (trauma > 0f)
        {
            float shakeAmount = Mathf.Pow(trauma, traumaExponent);

            if (shakeMode == ShakeMode.TRANSLATION_SHAKE || shakeMode == ShakeMode.FULL_SHAKE)
            {
                transform.position = originalPosition + new Vector3(
                    maxTranslationShake.x * Mathf.PerlinNoise(seed, Time.time * frequency) * 2,
                    maxTranslationShake.y * Mathf.PerlinNoise(seed + 1, Time.time * frequency) * 2,
                    maxTranslationShake.z * Mathf.PerlinNoise(seed + 2, Time.time * frequency) * 2
                ) * shakeAmount;
            }

            if (shakeMode == ShakeMode.ANGULAR_SHAKE || shakeMode == ShakeMode.FULL_SHAKE)
            {
                transform.rotation = Quaternion.Euler(new Vector3(
                    maxAngularShake.x * (Mathf.PerlinNoise(seed + 3, Time.time * frequency) * 2 - 1),
                    maxAngularShake.y * (Mathf.PerlinNoise(seed + 4, Time.time * frequency) * 2 - 1),
                    maxAngularShake.z * (Mathf.PerlinNoise(seed + 5, Time.time * frequency) * 2 - 1)
                ) * shakeAmount);
            }

            trauma = Mathf.Clamp01(trauma - recoverySpeed * Time.deltaTime);
        }
    }

    public void UpdatePosition(Vector3 currentPosition) { originalPosition = currentPosition; }

    public void InduceShake(float shakeAmount) { trauma = Mathf.Clamp01(trauma + shakeAmount); }
}
