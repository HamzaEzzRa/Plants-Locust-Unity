using UnityEngine;

[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    [SerializeField] private Light directionalLight;
    [SerializeField] private LightingPreset lightingPreset;

    [SerializeField, FloatRangeSlider(0, 24)] private FloatRange limit = new FloatRange(3, 21);

    [SerializeField] private bool isActive = default;

    public static LightingManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Update()
    {
        if (isActive)
        {
            if (lightingPreset == null)
            {
                return;
            }

            UpdateLighting(Mathf.Clamp(TimeManager.Instance.TimeOfDay, limit.Min, limit.Max) / 24f);
        }
    }

    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = lightingPreset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = lightingPreset.FogColor.Evaluate(timePercent);
        RenderSettings.fogDensity = lightingPreset.FogDensity.Evaluate(timePercent);

        if (directionalLight != null)
        {
            directionalLight.color = lightingPreset.DirectionalColor.Evaluate(timePercent);
            //Vector3 newRotation = new Vector3(Mathf.Clamp(((timePercent * 360f) - 90f), 0, 180), 225f, 0f);
            //directionalLight.transform.localRotation = Quaternion.Euler(newRotation);
        }
    }

    private void OnValidate()
    {
        if (directionalLight != null)
            return;

        if (RenderSettings.sun != null)
            directionalLight = RenderSettings.sun;
        else
        {
            Light[] lights = FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    directionalLight = light;
                    break;
                }
            }
        }
    }
}
