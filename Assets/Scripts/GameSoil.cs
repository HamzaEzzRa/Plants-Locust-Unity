using UnityEngine;

public class GameSoil : Interactable
{
    public int NumberOfActions => numberOfActions;

    [SerializeField, Range(1, 5)] private int numberOfActions = 3;
    [SerializeField] private GameObject seedPlantFx = default;

    public void Interact(int count, TileType seedType)
    {
        //Instantiate(seedPlantFx, transform.position + new Vector3(0f, 0.5f, 0f), Quaternion.identity);

        if (count >= numberOfActions)
        {
            Tile currentTile = GetComponent<TileContent>().tile;
            TileContent newContent = currentTile.Content.OriginFactory.Get(seedType);
            currentTile.Type = newContent.Type = seedType;
            Destroy(currentTile.Content);
            currentTile.ReplaceContent(newContent, reclaim:false);
        }
    }
}
