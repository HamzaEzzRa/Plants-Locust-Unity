using UnityEngine;

public abstract class GameBehavior : MonoBehaviour
{
    public virtual bool GameUpdate() => true;
}