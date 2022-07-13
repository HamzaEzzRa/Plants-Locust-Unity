using UnityEngine;

[ExecuteInEditMode]
public class GrassPointCloudRenderer : MonoBehaviour
{
    [SerializeField] private MeshFilter meshFilter;

    [SerializeField] private int seed;
    [SerializeField] private Vector2 size;
    [SerializeField, Range(1, 100000)] int grassNumber;

    [SerializeField] private float startHeight = 1000f, maxHeight = 1000f;
    [SerializeField] private float grassOffset = 0f;
    [SerializeField, Range(0.0001f, 1f)] private float uvScale = 0.001f;

    [SerializeField, HideInInspector] private Mesh pointCloudMesh;
    private const int GroundLayerMask = 1 << 8;

    public void GenerateCloudMesh()
    {
        Random.InitState(seed);
        Vector3[] positions = new Vector3[grassNumber];
        int[] indices = new int[grassNumber];
        Color[] colors = new Color[grassNumber];
        Vector3[] normals = new Vector3[grassNumber];
        Vector2[] uvs = new Vector2[grassNumber];
        for (int i = 0; i < grassNumber; i++)
        {
            Vector3 origin = transform.position;
            origin.y = startHeight;
            origin.x += size.x * Random.Range(-0.5f, 0.5f);
            origin.z += size.y * Random.Range(-0.5f, 0.5f);
            Ray ray = new Ray(origin, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, maxHeight, GroundLayerMask))
            {
                origin = hit.point;
                origin.y += grassOffset;
                positions[i] = origin;
                indices[i] = i;
                colors[i] = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
                normals[i] = hit.normal;
                Vector3 _ = (origin - transform.position) * uvScale;
                uvs[i] = new Vector2(_.x, _.z);
            }
        }
        pointCloudMesh = new Mesh();
        pointCloudMesh.SetVertices(positions);
        pointCloudMesh.SetIndices(indices, MeshTopology.Points, 0);
        pointCloudMesh.SetUVs(0, uvs);
        pointCloudMesh.SetNormals(normals);
        pointCloudMesh.SetColors(colors);
        meshFilter.mesh = pointCloudMesh;
    }
}
