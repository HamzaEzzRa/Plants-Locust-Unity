public class PlayerRunState : PlayerBaseState
{
    private bool lastGroundedState;
    private int lastEquippedId = -1;

    public override void EnterState(Player player)
    {
        lastGroundedState = player.IsGrounded;
        player.AnimationManager.ChangeAnimation(player.RUN_ANIMATION, player.MoveSpeed * 0.35f);
        if (!player.IsGrounded)
        {
            player.AnimationManager.PauseAnimation();
        }

        lastEquippedId = UpdateEquipLayers(player, lastEquippedId);
    }

    public override void UpdateState(Player player)
    {
        if (player.IsGrounded != lastGroundedState)
        {
            if (!player.IsGrounded)
            {
                player.AnimationManager.PauseAnimation();
            }
            else
            {
                player.AnimationManager.ResumeAnimation();
            }

            lastGroundedState = player.IsGrounded;
        }

        lastEquippedId = UpdateEquipLayers(player, lastEquippedId);

        if (!player.IsMoving && player.IsGrounded)
        {
            player.ChangeState(player.IDLE_STATE);
        }
    }
}
