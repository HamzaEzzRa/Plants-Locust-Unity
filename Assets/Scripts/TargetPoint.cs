using UnityEngine;

public class TargetPoint : MonoBehaviour
{
    public static int BufferedCount { get; private set; }

    public static TargetPoint RandomBuffered => GetBuffered(Random.Range(0, BufferedCount));

    private const int enemyLayerMask = 1 << 9;
    private static Collider[] buffer = new Collider[100];

    public Enemy Enemy => enemy;

    public Vector3 Position => transform.position;

    [SerializeField] private Enemy enemy;

    public static bool FillBuffer(Vector3 position, float range)
    {
        Vector3 top = position;
        top.y += 3f;
        BufferedCount = Physics.OverlapCapsuleNonAlloc(position, top, range, buffer, enemyLayerMask);
        return BufferedCount > 0;
    }

    public static TargetPoint GetBuffered(int index)
    {
        var target = buffer[index].GetComponent<TargetPoint>();
        return target;
    }
}