using UnityEngine;

[System.Serializable, CreateAssetMenu()]
public class LightingPreset : ScriptableObject
{
    public Gradient AmbientColor => ambientColor;

    public Gradient DirectionalColor => directionalColor;

    public Gradient FogColor => fogColor;

    public AnimationCurve FogDensity => fogDensity;

    [SerializeField] private Gradient ambientColor;
    [SerializeField] private Gradient directionalColor;
    [SerializeField] private Gradient fogColor;
    [SerializeField] private AnimationCurve fogDensity;
}
