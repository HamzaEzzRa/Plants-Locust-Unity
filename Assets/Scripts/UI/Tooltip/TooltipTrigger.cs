using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Serializable] public class TextCallback : SerializableCallback<string> { }

    [SerializeField] private string header;
    [SerializeField, HideInInspector] public string body;
    [SerializeField, HideInInspector] public TextCallback dynamicBody;
    [SerializeField] public bool isBodyDynamic;

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipManager.Instance.UpdatePosition(eventData.position);

        if (isBodyDynamic && dynamicBody != null)
        {
            body = dynamicBody.Invoke();
        }
        TooltipManager.Instance.ShowTooltip(body, header);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance.HideTooltip();
    }

    private void OnMouseEnter()
    {
        TooltipManager.Instance.UpdatePosition(Input.mousePosition);
        TooltipManager.Instance.ShowTooltip(body, header);
    }

    private void OnMouseExit()
    {
        TooltipManager.Instance.HideTooltip();
    }
}
