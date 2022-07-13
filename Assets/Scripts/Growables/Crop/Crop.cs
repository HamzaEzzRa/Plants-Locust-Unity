using UnityEngine;

public class Crop : Growable
{
    public GrowableBaseState<Crop> CurrentState => currentState;

    public bool IsWhole => timeToWhole <= 0f;
    public bool IsRotten => timeToRotten <= 0f;

    [HideInInspector] public float timeToWhole;
    [HideInInspector] public float timeToRotten;

    private GrowableBaseState<Crop> currentState;

    protected override void Start()
    {
        base.Start();
        if (growableData != null)
        {
            timeToWhole = growableData.TimeToWhole;
            timeToRotten = growableData.TimeToRotten;

            ChangeState(GrowableManager.Instance.CROP_GROWING_STATE);
        }
    }

    private void Update()
    {
        currentState.UpdateState(this);
    }

    public void ChangeState(GrowableBaseState<Crop> newState)
    {
        currentState = newState;
        currentState.EnterState(this);
    }
}
