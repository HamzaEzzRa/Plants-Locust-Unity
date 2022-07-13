using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class PoisonCloud : WarEntity
{
    [SerializeField, Range(0.5f, 3f)] private float cloudDuration = 0.5f;
    [SerializeField, Range(0.5f, 3f)] private float poisonDuration = 0.5f;

    private ParticleSystem particles;

    private float age;

    private void Awake()
    {
        particles = GetComponent<ParticleSystem>();
    }

    public void Initialize(Vector3 position, float radius, float dps, float poisonDuration)
    {
        TargetPoint.FillBuffer(position, radius);
        for (int i = 0; i < TargetPoint.BufferedCount; i++)
        {
            Enemy target = TargetPoint.GetBuffered(i).Enemy;
            if (target != null)
                StartCoroutine(target.ApplyContinuousDamage(dps, poisonDuration));
        }

        transform.localPosition = position;
        particles.Play();
    }

    public override bool GameUpdate()
    {
        age += Time.deltaTime;
        if (age >= cloudDuration && age >= poisonDuration)
        {
            OriginFactory.Reclaim(this);
            return false;
        }
        return true;
    }
}
