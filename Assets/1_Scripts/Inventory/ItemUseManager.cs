public class ItemUseManager : Singleton<ItemUseManager>
{
    public void Use(int itemId)
    {
        if (itemId == 0) return;

        var quantity = UserData.Instance.GetItemQuantity(itemId);
        if (quantity <= 0)
        {
            LogManager.Log($"[Item] Use: 보유 수량이 없음 (itemId={itemId})");
            return;
        }

        var itemData = GameData.Instance.GetItemData(itemId);
        switch (itemData.ItemType)
        {
            case ItemType.FamiliarCall:
            {
                if (!UserData.Instance.TrySummonFamiliar(itemId)) break;
                UserData.Instance.RemoveItem(itemId, 1);
                break;
            }
            default:
            {
                LogManager.Log($"[Item] Use: 사용할 수 없는 아이템 (itemId={itemId}, itemType={itemData.ItemType})");
                break;
            }
        }
    }
}
