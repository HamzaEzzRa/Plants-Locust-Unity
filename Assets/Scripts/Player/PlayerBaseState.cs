public abstract class PlayerBaseState
{
    public abstract void EnterState(Player player);

    public abstract void UpdateState(Player player);

    protected int UpdateEquipLayers(Player player, int lastEquippedId)
    {
        if (player.EquippedItem != null && player.EquippedItem.ItemData != null)
        {
            int equippedId = player.EquippedItem.ItemData.ID;
            if (lastEquippedId != equippedId)
            {
                lastEquippedId = equippedId;
                if (equippedId == 10003)
                {
                    player.AnimationManager.Animator.SetLayerWeight(player.HAND_LAYER, 0.7f);
                    player.AnimationManager.ChangeAnimation(player.BAG_HOLD_ANIMATION);
                }
                else if (equippedId == 10004)
                {
                    player.AnimationManager.Animator.SetLayerWeight(player.HAND_LAYER, 0.4f);
                    player.AnimationManager.ChangeAnimation(player.AXE_HOLD_ANIMATION);
                }
                else if (equippedId == 10006)
                {
                    player.AnimationManager.Animator.SetLayerWeight(player.HAND_LAYER, 0.4f);
                    player.AnimationManager.ChangeAnimation(player.HOE_HOLD_ANIMATION);
                }
                else if (equippedId == 10007)
                {
                    player.AnimationManager.Animator.SetLayerWeight(player.HAND_LAYER, 0.4f);
                    player.AnimationManager.ChangeAnimation(player.SICKLE_HOLD_ANIMATION);
                }
                else if (equippedId == 10008)
                {
                    player.AnimationManager.Animator.SetLayerWeight(player.HAND_LAYER, 1f);
                    player.AnimationManager.ChangeAnimation(player.SHOTGUN_HOLD_ANIMATION);
                }
            }
        }
        else
        {
            player.AnimationManager.ClearLayerWeights();
            lastEquippedId = -1;
        }

        return lastEquippedId;
    }

    protected void UpdateEquipment(Player player)
    {
        player.canSwitchEquipped = true;
        player.LastEquippedItem?.gameObject.SetActive(false);
        player.EquippedItem?.gameObject.SetActive(true);
    }
}
