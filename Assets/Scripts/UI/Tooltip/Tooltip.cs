using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode, RequireComponent(typeof(LayoutElement)), RequireComponent(typeof(RectTransform))]
public class Tooltip : MonoBehaviour
{
    public RectTransform RectTransform => rectTransform;
    public CanvasGroup CanvasGroup => canvasGroup;

    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private LayoutElement layoutElement;
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private TextMeshProUGUI headerField;
    [SerializeField] private TextMeshProUGUI bodyField;
    [SerializeField] private int characterWrapLimit;

    public void SetText(string body, string header = "")
    {
        if (string.IsNullOrEmpty(header))
        {
            headerField.gameObject.SetActive(false);
        }
        else
        {
            headerField.gameObject.SetActive(true);
            headerField.text = header;
        }

        bodyField.text = body;

        int headerLength = headerField.text.Length;
        int bodyLength = bodyField.text.Length;

        layoutElement.enabled = headerLength > characterWrapLimit || bodyLength > characterWrapLimit;
    }

    private void Update()
    {
        if (Application.isEditor)
        {
            int headerLength = headerField.text.Length;
            int bodyLength = bodyField.text.Length;

            layoutElement.enabled = headerLength > characterWrapLimit || bodyLength > characterWrapLimit;
        }
    }
}
