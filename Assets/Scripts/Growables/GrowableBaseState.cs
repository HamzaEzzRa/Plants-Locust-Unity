public abstract class GrowableBaseState<T> where T:Growable
{
    public abstract void EnterState(T growable);

    public abstract void UpdateState(T growable);
}
