using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways, RequireComponent(typeof(GridLayoutGroup), typeof(RectTransform))]
public class FlexibleLayout : MonoBehaviour
{
    [SerializeField] private float minHeight;

    private GridLayoutGroup layout;
    private RectTransform rectTransform;

    private void Start()
    {
        layout = GetComponent<GridLayoutGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnValidate()
    {
        if (layout == null)
        {
            layout = GetComponent<GridLayoutGroup>();
        }
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        float usedWidth = rectTransform.rect.width - layout.padding.left - layout.padding.right;
        int cellsPerRow = (int)(usedWidth / (layout.cellSize.x + layout.spacing.x));
        int numberOfRows = Mathf.CeilToInt(transform.childCount / (float)cellsPerRow);

        float requiredHeight = layout.padding.top + layout.padding.bottom;
        requiredHeight += numberOfRows * (layout.cellSize.y + layout.spacing.y) - layout.spacing.y;

        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, Mathf.Max(minHeight, requiredHeight));
    }
}
