using UnityEngine;

public class PoisonPlant : Plant
{
    [SerializeField, Range(0.25f, 2f)] private float cloudsPerSecond = 0.5f;
    [SerializeField, Range(0.5f, 3f)] private float poisonCloudRadius = 2f;
    [SerializeField, Range(1f, 100f)] private float damagePerSecond = 10f;
    [SerializeField, Range(1f, 100f)] private float poisonDuration = 5f;

    private float cloudProgress;

    private new void Awake()
    {
        base.Awake();
    }

    public override void GameUpdate()
    {
        cloudProgress += cloudsPerSecond * Time.deltaTime;
        while (cloudProgress >= 1f)
        {
            if (AcquireTarget(out TargetPoint _))
            {
                SpreadCloud();
                cloudProgress -= 1f;
            }
            else
                cloudProgress = 0f;
        }
    }

    private void SpreadCloud()
    {
        GameManager.SpawnCloud().Initialize(transform.position, poisonCloudRadius, damagePerSecond, poisonDuration);
    }
}
