using System.Collections.Generic;
using UnityEngine;

public class GrowableManager : SingletonMonoBehavior<GrowableManager>
{
    [SerializeField] private GrowableListSO growableListSO = default;

    private Dictionary<int, GrowableData> growableDataDictionary;

    public readonly int CARROT_GROWTH_ANIMATION = Animator.StringToHash("Carrot_Growth");
    public readonly int CARROT_ROT_ANIMATION = Animator.StringToHash("Carrot_Rot");

    public readonly CropGrowingState CROP_GROWING_STATE = new CropGrowingState();
    public readonly CropWholeState CROP_WHOLE_STATE = new CropWholeState();
    public readonly CropRottingState CROP_ROTTING_STATE = new CropRottingState();

    public readonly MonsterPlantGrowingState MONSTER_PLANT_GROWING_STATE = new MonsterPlantGrowingState();
    public readonly MonsterPlantWholeState MONSTER_PLANT_WHOLE_STATE = new MonsterPlantWholeState();

    protected override void Awake()
    {
        base.Awake();
        PopulateItemDictionary();
    }

    private void PopulateItemDictionary()
    {
        growableDataDictionary = new Dictionary<int, GrowableData>(growableListSO.GrowableList.Count);

        foreach (GrowableData data in growableListSO.GrowableList)
        {
            growableDataDictionary.Add(data.ID, data);
        }
    }

    public GrowableData GetGrowableData(int id)
    {
        growableDataDictionary.TryGetValue(id, out GrowableData growableData);
        return growableData;
    }
}
