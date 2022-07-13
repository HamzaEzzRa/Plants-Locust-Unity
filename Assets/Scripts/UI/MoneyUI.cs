using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class MoneyUI : MonoBehaviour
{
    [SerializeField] private Player player;

    private TextMeshProUGUI textMesh;
    private int targetValue;

    private LTDescr valueDescr;

    private void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        targetValue = player.CurrentMoney;
        SetText(targetValue);
    }

    private void SetText(float value)
    {
        textMesh.SetText(Extra.MoneyFormat(value));
    }

    private void OnMoneyUpdateEvent(int amount)
    {
        if (valueDescr != null)
        {
            LeanTween.cancel(valueDescr.uniqueId);
        }

        int originalValue = targetValue;
        targetValue += amount;
        valueDescr = LeanTween.value(originalValue, targetValue, Mathf.Min(2f, Mathf.Abs(amount) * 0.035f))
            .setOnUpdate((float value) =>
            {
                SetText(value);
            })
            .setOnComplete(() =>
            {
                SetText(targetValue);
                valueDescr = null;
            });
    }

    private void OnEnable()
    {
        EventHandler.MoneyUpdateEvent += OnMoneyUpdateEvent;
    }

    private void OnDisable()
    {
        EventHandler.MoneyUpdateEvent -= OnMoneyUpdateEvent;
    }
}
