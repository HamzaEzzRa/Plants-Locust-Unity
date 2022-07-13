using UnityEngine;

public class CropWholeState : GrowableBaseState<Crop>
{
    public override void EnterState(Crop crop) { }

    public override void UpdateState(Crop crop)
    {
        crop.timeToRotten -= Time.deltaTime / (TimeManager.Instance.MinutesPerCycle * 60 / 24f);
        if (crop.timeToRotten <= 0f)
        {
            crop.timeToRotten = 0f;
            crop.ChangeState(GrowableManager.Instance.CROP_ROTTING_STATE);
        }
    }
}
