namespace MiniShooter
{
    public class EpinephrineItemUse : InventoryItemUse
    {
        public override bool Use(OnlinePlayer player)
        {
            if (player.TryGetComponent(out OnlinePlayerCharacter onlinePlayerCharacter)
                && onlinePlayerCharacter.Current
                && onlinePlayerCharacter.Current.TryGetComponent(out PlayerCharacterVitals vitals))
            {
                return vitals.TryFreezeStamina();
            }

            return false;
        }
    }
}