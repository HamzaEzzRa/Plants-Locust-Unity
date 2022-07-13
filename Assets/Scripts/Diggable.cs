using UnityEngine;

public class Diggable : Interactable
{
    [SerializeField, Range(1, 5)] private int numberOfActions = 3;
    [SerializeField] private GameObject digFx = default;

    public void Interact(int count)
    {
        Instantiate(digFx, transform.position + new Vector3(0f, 0.1f, 0f), Quaternion.identity);

        if (count >= numberOfActions)
        {
            Tile currentTile = GetComponent<TileContent>().tile;
            TileContent newContent = currentTile.Content.OriginFactory.Get(TileType.CROP_TILE);
            currentTile.Type = newContent.Type = TileType.CROP_TILE;
            currentTile.ReplaceContent(newContent);

            TerrainModifier.Instance.ModifyHeight(currentTile.transform.position, 1, 1, 3, -1f);

            Destroy(gameObject);
        }
    }
}
