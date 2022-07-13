using UnityEngine;

public class FirePlant : Plant
{
    [SerializeField, Range(1f, 10f)] private float turnSpeed = 2.5f;
    [SerializeField, Range(1f, 100f)] private float damagePerSecond = 10f;
    [SerializeField] private Transform turret = default, beam = default;

    private LineRenderer beamLine;
    private TargetPoint target;
    private Vector3 beamScale;

    private new void Awake()
    {
        base.Awake();
        beamLine = beam.GetComponentInChildren<LineRenderer>();
        beamScale = beamLine.GetPosition(1);
    }

    public override void GameUpdate()
    {
        if (TrackTarget(ref target) || AcquireTarget(out target))
            Shoot();
        else
            beam.gameObject.SetActive(false);
    }

    private void Shoot()
    {
        if (!beam.gameObject.activeInHierarchy)
            beam.gameObject.SetActive(true);

        Vector3 point = target.Position;
        StartCoroutine(Extra.LerpLookAt(turret, point, turnSpeed));
        beam.localRotation = turret.localRotation;

        float d = Vector3.Distance(turret.position, point);
        beamScale.z = d;
        beamLine.SetPosition(1, beamScale);
        beam.localPosition = turret.localPosition + 0.2f * d * beam.forward;

        target.Enemy.ApplyDamage(damagePerSecond * Time.deltaTime);
    }
}
