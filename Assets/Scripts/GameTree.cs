using UnityEngine;

public class GameTree : Damageable
{
    private enum TreeType
    {
        TREE,
        TRUNK,
        STUMP
    }

    [SerializeField] private GameTree treeTrunk = default;
    [SerializeField] private GameTree treeStump = default;
    [SerializeField] private Item treeLog = default;

    [SerializeField] private GameObject treeHitFx = default;
    [SerializeField] private GameObject treeDestroyFx = default;
    [SerializeField] private MeshFilter trunkMeshFilter = default;

    [SerializeField] private TreeType type;

    [SerializeField, Range(0, 5)] private int generatedLogs = 3;

    private Shakeable shakeableComponent;

    private static LayerMask GROUND_LAYER = 1 << 8;

    private LTDescr autoDestroyDescr;

    private void Start()
    {
        shakeableComponent = gameObject.AddComponent<Shakeable>();
        shakeableComponent.Setup(Shakeable.ShakeMode.ANGULAR_SHAKE, Vector3.one * 1.5f, Vector3.one * 1.5f, 10f, 1.25f, 3f);
        shakeableComponent.UpdatePosition(transform.position);
    }

    private void GenerateLogs()
    {
        GameObject treeDestroyFxObject = Instantiate(treeDestroyFx, transform.position, transform.rotation);
        Vector3 newPosition = Vector3.zero;
        newPosition.y = trunkMeshFilter.mesh.bounds.size.y / 16f;
        Vector3 newScale = new Vector3(0.3f, newPosition.y, 0.3f);

        ParticleSystem.ShapeModule shape = treeDestroyFxObject.GetComponent<ParticleSystem>().shape;
        shape.position = newPosition;
        shape.scale = newScale;

        for (int i = 0; i < generatedLogs; i++)
        {
            Vector3 pos = transform.position + new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
            pos.y = treeLog.transform.position.y;
            Vector3 rot = treeLog.transform.rotation.eulerAngles + new Vector3(0f, 0f, Random.Range(-35f, 35f));
            Item log = Instantiate(treeLog, pos, Quaternion.Euler(rot));

            int originalLayer = log.gameObject.layer;
            log.gameObject.layer = 0;
            Vector3 originalScale = log.transform.localScale;
            log.transform.localScale = Vector3.zero;
            int rotationDir = Random.Range(0f, 1f) < 0.5f ? 1 : -1;

            LeanTween.value(0f, 1f, 0.3f)
                .setOnUpdate((float value) =>
                {
                    rot.z += rotationDir * 1f;
                    log.transform.localRotation = Quaternion.Euler(rot);
                    log.transform.localScale = value * originalScale;
                })
                .setOnComplete(() =>
                {
                    log.transform.localScale = originalScale;
                    log.gameObject.layer = originalLayer;
                });
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (type == TreeType.TRUNK && (1 << collision.gameObject.layer) == GROUND_LAYER)
        {
            LeanTween.cancel(autoDestroyDescr.uniqueId);
            GenerateLogs();
            Destroy(gameObject);
        }
    }

    public new void ApplyDamage(float value)
    {
        Instantiate(treeHitFx, transform.position + new Vector3(0f, 0.3f, 0f), Quaternion.identity);
        if (type == TreeType.TREE)
        {
            shakeableComponent.InduceShake(1f);
        }

        health -= value;
        if (health <= 0f)
        {
            if (type == TreeType.TREE)
            {
                Vector3 stumpPosition = transform.position;
                stumpPosition.y = treeStump.transform.position.y;

                Vector3 trunkPosition = transform.position;
                trunkPosition.y = treeTrunk.transform.position.y;
                Vector3 trunkRotation = treeTrunk.transform.rotation.eulerAngles + new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));

                GameTree stump = Instantiate(treeStump, stumpPosition, treeStump.transform.rotation);
                stump.transform.SetParent(transform.parent);

                GameTree trunk = Instantiate(treeTrunk, trunkPosition, Quaternion.Euler(trunkRotation));
                trunk.transform.SetParent(transform.parent);
                trunk.autoDestroyDescr = LeanTween.delayedCall(6f, () => {
                    trunk.GenerateLogs();
                    Destroy(trunk.gameObject);
                });

                Tile currentTile = GetComponent<TileContent>().tile;
                TileContent newContent = stump.gameObject.AddComponent<TileContent>();
                newContent.Type = TileType.TREE_TILE;
                newContent.OriginFactory = currentTile.Content.OriginFactory;
                currentTile.ReplaceContent(newContent);
            }
            else if (type == TreeType.STUMP)
            {
                GenerateLogs();

                Tile currentTile = GetComponent<TileContent>().tile;
                currentTile.ReplaceContent(currentTile.Content.OriginFactory.Get(TileType.EMPTY_TILE));
                currentTile.Content.Type = currentTile.Type = TileType.EMPTY_TILE;
            }
            Destroy(gameObject);
        }

        DamagePopup.Create(value, transform.position + new Vector3(1f, 1f, 0f));
    }
}
