using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Player Player => player;
    public GameBoard Board => board;
    public TileContentFactory TileContentFactory => tileContentFactory;

    [SerializeField] private Player player = default;
    [SerializeField] private GameBoard board = default;
    [SerializeField] private TileContentFactory tileContentFactory = default;
    [SerializeField] private EnemyFactory enemyFactory = default;
    [SerializeField] private WarFactory warFactory = default;

    [SerializeField, Range(0.1f, 10f)] private float enemySpawnSpeed = 1f;
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private ShopManager shopManager;

    private float spawnProgress;
    private GameBehaviorCollection enemies = new GameBehaviorCollection();
    private GameBehaviorCollection nonEnemies = new GameBehaviorCollection();

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        board.GameUpdate();
        board.UpdateTiles();
        //board.Initialize(tileContentFactory);
    }

    private void OnEnable()
    {
        Instance = this;
    }

    private void Update()
    {
        inputHandler.HandleInput(player, board, shopManager);

        spawnProgress += enemySpawnSpeed * Time.deltaTime;
        while (spawnProgress >= 1f)
        {
            spawnProgress -= 1f;
            SpawnEnemy();
        }
        enemies.GameUpdate();
        Physics.SyncTransforms();
        board.GameUpdate();
        nonEnemies.GameUpdate();
    }

    private void SpawnEnemy()
    {
        if (board.SpawnPoints.Count > 0)
        {
            int i = 0, index = Random.Range(0, board.SpawnPoints.Count);
            Tile spawnPoint = null;
            foreach (Tile tile in board.SpawnPoints)
            {
                if (i == index)
                {
                    spawnPoint = tile;
                    break;
                }
                i++;
            }
            if (spawnPoint == null)
            {
                return;
            }

            Enemy enemy = enemyFactory.Get();
            enemy.SpawnOn(spawnPoint);
            enemies.Add(enemy);
        }
    }

    public static Shell SpawnShell()
    {
        Shell shell = Instance.warFactory.Shell;
        Instance.nonEnemies.Add(shell);
        return shell;
    }

    public static Explosion SpawnExplosion()
    {
        Explosion explosion = Instance.warFactory.Explosion;
        Instance.nonEnemies.Add(explosion);
        return explosion;
    }

    public static PoisonCloud SpawnCloud()
    {
        PoisonCloud cloud = Instance.warFactory.PoisonCloud;
        Instance.nonEnemies.Add(cloud);
        return cloud;
    }
}
