using UnityEngine;

public class Shell : WarEntity
{
    private Vector3 launchPoint, targetPoint, launchVelocity;

    private float age, blastRadius, damage;

    private float inverseShakeRadius, maximumShake;

    public void Initialize(Vector3 launchPoint, Vector3 targetPoint, Vector3 launchVelocity, float blastRadius, float damage, float shakeRadius, float maximumShake)
    {
        this.launchPoint = launchPoint;
        this.targetPoint = targetPoint;
        this.launchVelocity = launchVelocity;
        this.blastRadius = blastRadius;
        this.damage = damage;
        this.maximumShake = maximumShake;
        inverseShakeRadius = 1f / shakeRadius;
        age = 0f;
    }

    public override bool GameUpdate()
    {
        age += Time.deltaTime;
        Vector3 position = launchPoint + launchVelocity * age;
        if (float.IsNaN(position.x) || float.IsNaN(position.y) || float.IsNaN(position.z))
        {
            return false;
        }

        position.y -= 0.5f * 9.81f * age * age;
        if (position.y <= targetPoint.y)
        {
            GameManager.SpawnExplosion().Initialize(targetPoint, blastRadius, damage);
            ShakeTarget();
            OriginFactory.Reclaim(this);
            return false;
        }
        transform.localPosition = position;
        Vector3 direction = launchVelocity;
        direction.y -= 9.81f * age;
        if (direction.sqrMagnitude >= 0.001f)
        {
            transform.localRotation = Quaternion.LookRotation(direction);
        }

        return true;
    }

    private void ShakeTarget()
    {
        float distance = Vector3.Distance(transform.position, Shakeable.CamInstance.transform.position);
        float distance01 = Mathf.Clamp01(distance * inverseShakeRadius);

        float shakeAmount = (1 - Mathf.Pow(distance01, 2)) * maximumShake;

        Shakeable.CamInstance.InduceShake(shakeAmount);
    }
}
