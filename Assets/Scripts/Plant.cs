using UnityEngine;

public abstract class Plant : TileContent
{
    [SerializeField, Range(1.5f, 10.5f)] protected float targetingRange = 1.5f;

    protected bool AcquireTarget(out TargetPoint target)
    {
        if (TargetPoint.FillBuffer(transform.localPosition, targetingRange))
        {
            target = TargetPoint.RandomBuffered;
            return true;
        }
        target = null;
        return false;
    }

    protected bool TrackTarget(ref TargetPoint target)
    {
        if (target == null)
            return false;
        float x = transform.localPosition.x - target.Position.x;
        float z = transform.localPosition.z - target.Position.z;
        float r = targetingRange + 0.125f * target.Enemy.Scale;
        if (x * x + z * z > r * r)
        {
            target = null;
            return false;
        }

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 position = transform.localPosition;
        position.y += 0.01f;
        Gizmos.DrawWireSphere(position, targetingRange);
    }
}
