using UnityEngine;

public abstract class WarEntity : GameBehavior
{
    private WarFactory originFactory;

    public WarFactory OriginFactory
    {
        get => originFactory;
        set
        {
            if (originFactory != null)
                return;
            originFactory = value;
        }
    }

    public void Recycle() { originFactory.Reclaim(this); }
}