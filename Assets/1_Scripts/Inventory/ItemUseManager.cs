public class ItemUseManager : Singleton<ItemUseManager>
{
    public void Use(int itemId)
    {
        if (itemId == 0) return;

        var quantity = UserData.Instance.GetItemQuantity(itemId);
        if (quantity <= 0)
        {
            GlobalManager.Instance.ShowToastMessage(LocalizationKey.InventoryPopup_ItemCountZero.Text());
            return;
        }

        var itemData = GameData.Instance.GetItemData(itemId);
        switch (itemData.ItemType)
        {
            case ItemType.FamiliarCall:
            {
                if (!UserData.Instance.CanAddFamiliar(itemId)) break;

                var familiarCallData = GameData.Instance.GetFamiliarCallData(itemId);
                var parameter = new SystemTwoButtonParameter
                {
                    Description = LocalizationKey.Familiar_CallMessage.Text(itemData.Name, familiarCallData.Name),
                    ConfirmButtonText = LocalizationKey.ConfirmButton,
                    CancelButtonText = LocalizationKey.CancelButton,
                    OnConfirm = () =>
                    {
                        if (!UserData.Instance.TryAddFamiliar(itemId)) return;
                        UserData.Instance.RemoveItem(itemId, 1);
                    },
                };
                PopupManager.Instance.ShowPopup(PopupManager.Type.SystemButton, parameter);
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
