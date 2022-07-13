using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Fadeable : MonoBehaviour
{
    public static readonly int URP_COLOR_PROPERTY = Shader.PropertyToID("_BaseColor");

    public bool IsFaded => isFaded;

    [SerializeField] private Material opaqueMat, transparentMat;
    [SerializeField, Range(0.1f, 5f)] private float fadeSpeed = 1f;
    [SerializeField, FloatRangeSlider(0f, 1f)] private FloatRange fadeClamp = new FloatRange(0.7f, 1f);

    private Material opaqueMatInstance, transparentMatInstance;
    private bool isFaded;

    private void Start()
    {
        opaqueMatInstance = new Material(opaqueMat);
        transparentMatInstance = new Material(transparentMat);
    }

    public IEnumerator FadeOut()
    {
        if (!isFaded)
        {
            isFaded = true;
            GetComponent<Renderer>().material = transparentMatInstance;
            Color color = GetComponent<Renderer>().material.GetColor(URP_COLOR_PROPERTY);
            while (color.a > fadeClamp.Min)
            {
                color.a -= fadeSpeed * Time.deltaTime;
                GetComponent<Renderer>().material.SetColor(URP_COLOR_PROPERTY, color);
                yield return null;
            }
        }
    }

    public IEnumerator FadeIn()
    {
        if (isFaded)
        {
            isFaded = false;
            Color color = GetComponent<Renderer>().material.GetColor(URP_COLOR_PROPERTY);
            while (color.a < fadeClamp.Max)
            {
                color.a += fadeSpeed * Time.deltaTime;
                GetComponent<Renderer>().material.SetColor(URP_COLOR_PROPERTY, color);
                yield return null;
            }
            GetComponent<Renderer>().material = opaqueMatInstance;
        }
    }
}
