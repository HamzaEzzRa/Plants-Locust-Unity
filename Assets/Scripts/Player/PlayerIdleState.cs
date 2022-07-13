public class PlayerIdleState : PlayerBaseState
{
    private int lastEquippedId = -1;

    public override void EnterState(Player player)
    {
        player.AnimationManager.ChangeAnimation(player.IDLE_ANIMATION);

        lastEquippedId = UpdateEquipLayers(player, lastEquippedId);
    }

    public override void UpdateState(Player player)
    {
        lastEquippedId = UpdateEquipLayers(player, lastEquippedId);

        if (player.IsMoving)
        {
            player.ChangeState(player.RUN_STATE);
        }
    }
}
