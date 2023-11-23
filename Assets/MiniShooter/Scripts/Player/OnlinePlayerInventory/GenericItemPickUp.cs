using Mirror;

namespace MiniShooter
{
    public class GenericItemPickUp : InventoryItemPickUp
    {
        [Server]
        public override void PickUp(PlayerCharacter character)
        {
            if (character.ServerPlayerCharacter.TryGetComponent(out OnlinePlayerInventory inventory))
            {
                inventory.PickupItem(inventoryItem);
                inventoryItem.PickUp();
            }
        }
    }
}