using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance { get; private set; }

    [SerializeField] private Tooltip tooltip;

    [SerializeField, Range(0f, 2f)] private float delay = 0.5f;

    private LTDescr showDescr, fadeDescr;

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

    public void UpdatePosition(Vector2 position)
    {
        Vector2 pivot = new Vector2(position.x / Screen.width, position.y / Screen.height);
        tooltip.RectTransform.pivot = pivot;

        tooltip.transform.position = position;
    }

    public void ShowTooltip(string body, string header = "")
    {
        tooltip.SetText(body, header);

        if (fadeDescr != null)
        {
            LeanTween.cancel(fadeDescr.uniqueId);

            tooltip.gameObject.SetActive(true);
            float currentValue = tooltip.CanvasGroup.alpha;
            fadeDescr = LeanTween.value(currentValue, 1f, 0.25f)
                .setOnUpdate((float value) => tooltip.CanvasGroup.alpha = value);
        }
        else
        {
            showDescr = LeanTween.delayedCall(delay, () =>
            {
                if (tooltip.CanvasGroup != null)
                {
                    tooltip.CanvasGroup.alpha = 0f;
                }
                tooltip.gameObject.SetActive(true);
                fadeDescr = LeanTween.value(0f, 1f, 0.25f)
                    .setOnUpdate((float value) => tooltip.CanvasGroup.alpha = value);
            });
        }
    }

    public void HideTooltip()
    {
        if (fadeDescr != null)
        {
            LeanTween.cancel(fadeDescr.uniqueId);
        }
        if (showDescr != null)
        {
            LeanTween.cancel(showDescr.uniqueId);
        }

        fadeDescr = LeanTween.value(1f, 0f, 0.25f)
            .setOnUpdate((float value) => tooltip.CanvasGroup.alpha = value)
            .setOnComplete(() => {
                tooltip.gameObject.SetActive(false);
                fadeDescr = null;
            });
    }
}
