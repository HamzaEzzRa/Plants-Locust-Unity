using UnityEngine;

public class MonsterPlant : Growable
{
    public GrowableBaseState<MonsterPlant> CurrentState => currentState;

    public bool IsWhole => timeToWhole <= 0f;
    public float Hostility => 1f - timeToHostile / growableData.TimeToHostile;

    [HideInInspector] public float timeToWhole;
    [HideInInspector] public float timeToHostile;

    private GrowableBaseState<MonsterPlant> currentState;

    protected override void Start()
    {
        base.Start();
        if (growableData != null)
        {
            timeToWhole = growableData.TimeToWhole;
            timeToHostile = growableData.TimeToHostile;

            ChangeState(GrowableManager.Instance.MONSTER_PLANT_GROWING_STATE);
        }
    }

    private void Update()
    {
        currentState.UpdateState(this);
    }

    public void ChangeState(GrowableBaseState<MonsterPlant> newState)
    {
        currentState = newState;
        currentState.EnterState(this);
    }
}
