using PlayerScripts;

namespace GamePlay.Collectibles
{
    public class Axe : CollectibleBase
    {
        protected override void OnCollected(PlayerInventory inventory)
        {
            inventory.SetHasAxe(true);
        }
    }
}