using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SlidingBar : MonoBehaviour
{
    [SerializeField] private Camera targetCamera = default;
    [SerializeField] private Vector3 offset = default;

    private Slider slider;
    private float maxValue, currentValue;
    private Transform target;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void LateUpdate()
    {
        transform.position = target.position + offset;
        transform.LookAt(transform.position + targetCamera.transform.forward);
    }

    public void Initialize(Transform target, float maxValue)
    {
        slider.value = 1f;
        this.target = target;
        this.maxValue = maxValue;
        currentValue = maxValue;
    }

    public void ChangeBarValue(float value)
    {
        currentValue += value;
        slider.value = Mathf.Clamp(currentValue / maxValue, 0f, 1f);
    }
}
