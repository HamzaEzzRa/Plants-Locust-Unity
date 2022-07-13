using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    private static int sortingOrder = 0;

    public static void Create(float damageValue, Vector3 startPosition, float fadeTime=2f)
    {
        GameObject newPopupObject = new GameObject("Damage Popup");
        newPopupObject.AddComponent<DamagePopup>();
        TextMeshPro newPopup = newPopupObject.AddComponent<TextMeshPro>();
        newPopup.fontMaterial.shader = Shader.Find("TextMeshPro/Distance Field Overlay");
        newPopup.transform.LookAt(newPopup.transform.position + Camera.main.transform.forward);
        newPopup.fontSize = 3.5f;
        newPopup.rectTransform.sizeDelta = Vector2.one;
        newPopup.transform.position = startPosition;

        newPopup.outlineWidth = 0.15f;
        newPopup.outlineColor = Color.black;
        newPopup.sortingOrder = sortingOrder;
        newPopup.SetText(damageValue.ToString());
        sortingOrder++;

        LeanTween.value(0f, 1f, fadeTime)
            .setOnUpdate((float value) =>
            {
                Color tmp = newPopup.color;
                tmp.a = 1 - value;
                newPopup.color = tmp;

                newPopup.transform.position += new Vector3(0f, 1f, 1f) * 0.005f;
                newPopup.transform.localScale += Vector3.one * 0.002f * (value < 0.3f ? 1 : -1);
            })
            .setOnComplete(() =>
            {
                Destroy(newPopupObject);
            })
            .setEaseOutQuad();
    }
}
