using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Explosion : WarEntity
{
    [SerializeField, Range(0.5f, 3f)] private float duration = 0.5f;

    private ParticleSystem particles;

    private float age;

    private void Awake()
    {
        particles = GetComponent<ParticleSystem>();
    }

    public void Initialize(Vector3 position, float blastRadius, float damage)
    {
        TargetPoint.FillBuffer(position, blastRadius);
        for (int i = 0; i < TargetPoint.BufferedCount; i++)
            TargetPoint.GetBuffered(i).Enemy.ApplyDamage(damage);
        
        transform.localPosition = position;
        particles.Play();
    }
    
    public override bool GameUpdate()
    {
        age += Time.deltaTime;
        if (age >= duration)
        {
            OriginFactory.Reclaim(this);
            return false;
        }
        return true;
    }
}
