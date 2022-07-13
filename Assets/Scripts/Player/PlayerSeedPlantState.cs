public class PlayerSeedPlantState : PlayerBaseState
{
    public override void EnterState(Player player)
    {
        player.AnimationManager.Animator.SetLayerWeight(player.HAND_LAYER, 0f);
        player.AnimationManager.ChangeAnimation(player.SEED_PLANT_ANIMATION);
    }

    public override void UpdateState(Player player)
    {
        if (!player.HasTarget && !player.AnimationManager.IsPlaying)
        {
            player.ChangeState(player.IDLE_STATE);
            UpdateEquipment(player);
        }
        else if (player.IsMoving)
        {
            player.ChangeState(player.RUN_STATE);
            UpdateEquipment(player);
        }
    }
}
