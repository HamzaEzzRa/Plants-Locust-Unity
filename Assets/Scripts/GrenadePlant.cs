using UnityEngine;

public class GrenadePlant : Plant
{
    [SerializeField] private Transform launcher = default;

    [SerializeField, Range(0.5f, 2f)] private float shotsPerSecond = 1f;
    [SerializeField, Range(1f, 10f)] private float turnSpeed = 2f;
    [SerializeField, Range(0.5f, 3f)] private float shellBlastRadius = 1f;
    [SerializeField, Range(1f, 100f)] private float shellDamage = 20f;
    [SerializeField, Range(0.1f, 100f)] private float shellShakeRadius = 20f;
    [SerializeField, Range(0.1f, 1f)] private float shellMaximumShake = 1f;

    private float launchSpeed, launchProgress;

    private new void Awake()
    {
        base.Awake();
        OnValidate();
    }

    public override void GameUpdate()
    {
        launchProgress += shotsPerSecond * Time.deltaTime;
        while (launchProgress >= 1f)
        {
            if (AcquireTarget(out TargetPoint target))
            {
                Launch(target);
                launchProgress -= 1f;
            }
            else
            {
                launchProgress = 0.999f;
            }
        }
    }

    private void Launch(TargetPoint target)
    {
        Vector3 launchPoint = launcher.position;
        Vector3 targetPoint = target.Position;
        targetPoint.y = 1f;

        Vector2 dir;
        dir.x = targetPoint.x - launchPoint.x;
        dir.y = targetPoint.z - launchPoint.z;
        float x = dir.magnitude;
        float y = -launchPoint.y;
        dir /= x;

        float g = 9.81f;
        float launchSpeedSquared = launchSpeed * launchSpeed;
        float r = launchSpeedSquared * launchSpeedSquared - g * (g * x * x + 2f * y * launchSpeedSquared);
        float tanTheta = (launchSpeedSquared + Mathf.Sqrt(r)) / (g * x);
        float cosTheta = Mathf.Cos(Mathf.Atan(tanTheta));
        float sinTheta = cosTheta * tanTheta;

        Vector3 lookPoint = new Vector3(dir.x, tanTheta, dir.y);
        if (lookPoint.sqrMagnitude >= 0.001f)
        {
            StartCoroutine(Extra.LerpLookRotate(launcher, lookPoint, turnSpeed));
        }

        GameManager.SpawnShell().Initialize(launchPoint, targetPoint, new Vector3(launchSpeed * cosTheta * dir.x, launchSpeed * sinTheta, launchSpeed * cosTheta * dir.y),
            shellBlastRadius, shellDamage, shellShakeRadius, shellMaximumShake);
    }

    private void OnValidate()
    {
        if (launcher != null)
        {
            float x = targetingRange + 0.25001f;
            float y = -launcher.position.y;
            launchSpeed = Mathf.Sqrt(9.81f * (y + Mathf.Sqrt(x * x + y * y)));
        }
    }
}
