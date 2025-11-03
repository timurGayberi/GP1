using Interfaces;
using UnityEngine;
using PlayerScripts;

namespace GamePlay.Collectibles
{
    public class Trash : CollectibleBase
    {
        protected override void OnCollected(PlayerInventory inventory)
        {
            //inventory.SetHasTrash(true);
        }
    }
}