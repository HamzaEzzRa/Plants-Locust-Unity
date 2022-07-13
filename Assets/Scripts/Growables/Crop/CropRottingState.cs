using UnityEngine;

public class CropRottingState : GrowableBaseState<Crop>
{
    public override void EnterState(Crop crop)
    {
        crop.AnimationManager.ChangeAnimation(GrowableManager.Instance.CARROT_ROT_ANIMATION);
    }

    public override void UpdateState(Crop crop) { }
}
