using UnityEngine;

[RequireComponent(typeof(Terrain))]
public class TerrainModifier : SingletonMonoBehavior<TerrainModifier>
{
    private Terrain terrain;

    private int heightMapResolution;
    private int alphaMapWidth, alphaMapHeight;
    private float[,] heightsBackup, heights;
    private float[,,] alphaMapsBackup;

    private void Start()
    {
        terrain = GetComponent<Terrain>();

        heightMapResolution = terrain.terrainData.heightmapResolution;
        alphaMapWidth = terrain.terrainData.alphamapWidth;
        alphaMapHeight = terrain.terrainData.alphamapHeight;

        heights = heightsBackup = terrain.terrainData.GetHeights(0, 0, heightMapResolution, heightMapResolution);
        alphaMapsBackup = terrain.terrainData.GetAlphamaps(0, 0, alphaMapWidth, alphaMapHeight);
    }

    public void ModifyHeight(Vector3 center, int width, int height, int smoothness, float value)
    {
        Vector3 localCenter = terrain.transform.InverseTransformPoint(center);

        width += smoothness + 1;
        height += smoothness + 1;

        int localX = Mathf.Clamp((int)(localCenter.x / terrain.terrainData.size.x * heightMapResolution - width / 2), 0, heightMapResolution);
        int localZ = Mathf.Clamp((int)(localCenter.z / terrain.terrainData.size.z * heightMapResolution - height / 2), 0, heightMapResolution);

        float[,] newHeights = terrain.terrainData.GetHeights(localX, localZ, width, height);
        float y = newHeights[width / 2, height / 2] + value;

        for (int s = 1; s <= smoothness + 1; s++)
        {
            float factor = s / (smoothness + 1);
            for (int x = Mathf.Max(0, s / 2); x < Mathf.Min(width - s / 2, heightMapResolution); x++)
            {
                for (int z = Mathf.Max(0, s / 2); z < Mathf.Min(height - s / 2, heightMapResolution); z++)
                {
                    newHeights[x, z] = Mathf.Clamp(factor * y, 0f, 1f);
                }
            }
        }

        terrain.terrainData.SetHeights(localX, localZ, newHeights);
    }

    private void OnDisable()
    {
        if (terrain)
        {
            terrain.terrainData.SetHeights(0, 0, heightsBackup);
            terrain.terrainData.SetAlphamaps(0, 0, alphaMapsBackup);
        }
    }
}
