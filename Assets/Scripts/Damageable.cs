using UnityEngine;

public abstract class Damageable : Interactable
{
    public float Health => health;
    [SerializeField, Range(0f, 1000f)] protected float health = 100f;

    public void ApplyDamage(float value) { health -= value; }
}
