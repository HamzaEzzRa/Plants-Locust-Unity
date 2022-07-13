using UnityEngine;
using System.Collections.Generic;

public class Interactable: GameBehavior
{
    public float InteractionRadius => interactionRadius;

    [ItemDescription, SerializeField] private List<int> interactionItemIds;
    [SerializeField, Range(0.1f, 10f)] protected float interactionRadius = 3f;
    private const int PlayerMask = 1 << 10;

    public void Interact()
    {
        return;
    }

    public bool CheckPlayerClose()
    {
        return Physics.CheckSphere(transform.position, interactionRadius, PlayerMask);
    }
    
    public bool CheckPlayerItem(int playerEquippedId)
    {
        return interactionItemIds.Contains(playerEquippedId);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
