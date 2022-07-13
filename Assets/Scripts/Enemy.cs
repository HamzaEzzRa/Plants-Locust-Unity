using System.Collections;
using UnityEngine;

public class Enemy : Damageable
{
    public float Scale { get; private set; }

    public EnemyFactory OriginFactory
    {
        get => originFactory;
        set
        {
            if (originFactory != null)
                return;
            originFactory = value;
        }
    }

    [SerializeField] private Transform model = default;
    [SerializeField] private SlidingBar healthBar;

    private EnemyFactory originFactory;

    private Tile tileFrom, tileTo;
    private Vector3 positionFrom, positionTo;
    private float
        progress,
        progressFactor,
        directionAngleFrom,
        directionAngleTo,
        pathOffset,
        speed;

    private Direction direction;
    private DirectionChange directionChange;

    public void Initialize(float scale, float speed, float pathOffset)
    {
        Scale = scale;
        model.localScale = new Vector3(scale, scale, scale);
        this.speed = speed;
        this.pathOffset = pathOffset;
        health = 100f * scale;
        healthBar.Initialize(model.transform, Health);
    }

    public override bool GameUpdate()
    {
        if (Health <= 0f)
        {
            OriginFactory.Reclaim(this);
            return false;
        }
        progress += Time.deltaTime * progressFactor;
        while (progress >= 1f)
        {
            if (tileTo == null)
            {
                OriginFactory.Reclaim(this);
                return false;
            }
            progress = (progress - 1f) / progressFactor;
            PrepareNextState();
            progress *= progressFactor;
        }
        if (directionChange == DirectionChange.None)
            transform.localPosition = Vector3.LerpUnclamped(positionFrom, positionTo, progress);
        else
        {
            float angle = Mathf.LerpUnclamped(directionAngleFrom, directionAngleTo, progress);
            transform.localRotation = Quaternion.Euler(0f, angle, 0f);
        }
        return true;
    }

    public void SpawnOn(Tile tile)
    {
        if (tile == null || tile.NextTileOnPath == null)
            return;

        tileFrom = tile;
        tileTo = tile.NextTileOnPath;
        progress = 0f;
        PrepareIntro();
    }

    public new void ApplyDamage(float value)
    {
        base.ApplyDamage(value);
        healthBar.ChangeBarValue(-value);
    }

    public IEnumerator ApplyContinuousDamage(float dps, float duration)
    {
        float progress = 0f;
        while (progress < duration)
        {
            ApplyDamage(dps * Time.deltaTime);
            progress += Time.deltaTime;
            yield return null;
        }
    }

    private void PrepareIntro()
    {
        positionFrom = tileFrom.transform.localPosition;
        positionTo = tileFrom.ExitPoint;
        direction = tileFrom.PathDirection;
        directionChange = DirectionChange.None;
        directionAngleFrom = directionAngleTo = direction.GetAngle();
        model.localPosition = new Vector3(pathOffset, 0f);
        transform.localRotation = direction.GetRotation();
        progressFactor = 2f * speed;
    }

    private void PrepareNextState()
    {
        tileFrom = tileTo;
        tileTo = tileTo.NextTileOnPath;
        positionFrom = positionTo;
        if (tileTo == null)
        {
            PrepareOutro();
            return;
        }
        positionTo = tileFrom.ExitPoint;
        directionChange = direction.GetDirectionChangeTo(tileFrom.PathDirection);
        direction = tileFrom.PathDirection;
        directionAngleFrom = directionAngleTo;

        switch (directionChange)
        {
            case DirectionChange.None: PrepareForward(); break;
            case DirectionChange.TurnRight: PrepareTurnRight(); break;
            case DirectionChange.TurnLeft: PrepareTurnLeft(); break;
            default: PrepareTurnAround(); break;
        }
    }

    private void PrepareForward()
    {
        transform.localRotation = direction.GetRotation();
        directionAngleTo = direction.GetAngle();
        model.localPosition = new Vector3(pathOffset, 0f);
        progressFactor = speed;
    }

    private void PrepareTurnRight()
    {
        directionAngleTo = directionAngleFrom + 90f;
        model.localPosition = new Vector3(pathOffset - 0.5f, 0f);
        transform.localPosition = positionFrom + direction.GetHalfVector();
        progressFactor = speed / (Mathf.PI * 0.5f * (0.5f - pathOffset));
    }

    private void PrepareTurnLeft() {
        directionAngleTo = directionAngleFrom - 90f;
        model.localPosition = new Vector3(pathOffset + 0.5f, 0f);
        transform.localPosition = positionFrom + direction.GetHalfVector();
        progressFactor = speed / (Mathf.PI * 0.5f * (0.5f + pathOffset));
    }
    
    private void PrepareTurnAround() {
        directionAngleTo = directionAngleFrom + (pathOffset < 0f ? 180f : -180f);
        model.localPosition = new Vector3(pathOffset, 0f);
        transform.localPosition = positionFrom;
        progressFactor = speed / (Mathf.PI * Mathf.Max(Mathf.Abs(pathOffset), 0.2f));
    }

    private void PrepareOutro()
    {
        positionTo = tileFrom.transform.localPosition;
        directionChange = DirectionChange.None;
        directionAngleTo = direction.GetAngle();
        model.localPosition = new Vector3(pathOffset, 0f);
        transform.localRotation = direction.GetRotation();
        progressFactor = 2f * speed;
    }
}