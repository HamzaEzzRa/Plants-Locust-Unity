using UnityEngine;

[RequireComponent(typeof(Player))]
public class ItemPicker : MonoBehaviour
{
    [SerializeField, Range(0.1f, 5f)] private float pickingSpeed = 1f;
    [SerializeField, Range(0.1f, 5f)] private float pickUpRadius = 1f;
    [SerializeField] private LayerMask ItemLayerMask = 1 << 12;

    private Player playerRef;

    private Collider[] overlapBuffer = new Collider[100];

    private float inversePickingSpeed;

    public float PickUpRadius => pickUpRadius;

    private void Start()
    {
        playerRef = GetComponent<Player>();
        inversePickingSpeed = 1f / pickingSpeed;
    }

    private void Update()
    {
        if (!InventoryManager.Instance.IsPlayerAtFullCapacity)
        {
            int overlapCount = Physics.OverlapSphereNonAlloc(playerRef.transform.position, pickUpRadius, overlapBuffer, ItemLayerMask);
            for (int i = 0; i < overlapCount; i++)
            {
                Collider collider = overlapBuffer[i];
                Item item = collider.GetComponent<Item>();

                if (item != null && item.ItemData != null && item.ItemData.IsPickable && !item.IsBeingPicked)
                {
                    InventoryManager.Instance.AddItem(InventoryLocation.PLAYER, item, item.gameObject, playerRef.transform, inversePickingSpeed);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (playerRef)
        {
            Gizmos.DrawWireSphere(playerRef.transform.position, pickUpRadius);
        }
    }
}
