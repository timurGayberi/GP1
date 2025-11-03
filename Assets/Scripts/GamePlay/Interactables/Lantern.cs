using PlayerScripts;

namespace GamePlay.Collectibles
{
    public class Lantern : CollectibleBase
    {
        protected override void OnCollected(PlayerInventory inventory)
        {
            inventory.SetHasLantern(true);
        }
    }
}