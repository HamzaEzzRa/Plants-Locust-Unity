using UnityEngine;

public class CropGrowingState : GrowableBaseState<Crop>
{
    public override void EnterState(Crop crop)
    {
        crop.AnimationManager.ControlAnimation(GrowableManager.Instance.CARROT_GROWTH_ANIMATION, 0f);
    }

    public override void UpdateState(Crop crop)
    {
        crop.timeToWhole -= Time.deltaTime / (TimeManager.Instance.MinutesPerCycle * 60 / 24f);
        if (crop.timeToWhole <= 0f)
        {
            crop.timeToWhole = 0f;
            crop.ChangeState(GrowableManager.Instance.CROP_WHOLE_STATE);
        }

        crop.AnimationManager.ControlAnimation(GrowableManager.Instance.CARROT_GROWTH_ANIMATION, 1f - crop.timeToWhole / crop.GrowableData.TimeToWhole);
    }
}
