using System.Collections.Generic;

public partial class UserData // Shop
{
    public int GetPurchasedCountToday(ShopType shopType, int itemId)
    {
        if (!_userDataInfo.ShopPurchaseRecords.TryGetValue(CurrentDay, out var shopDict)) return 0;
        if (!shopDict.TryGetValue(shopType, out var items)) return 0;
        foreach (var item in items)
        {
            if (item.ItemId != itemId) continue;
            return item.Quantity;
        }
        return 0;
    }

    public int GetPurchasedCountTotal(ShopType shopType, int itemId)
    {
        var sum = 0;
        foreach (var (_, shopDict) in _userDataInfo.ShopPurchaseRecords)
        {
            if (!shopDict.TryGetValue(shopType, out var items)) continue;
            foreach (var item in items)
            {
                if (item.ItemId != itemId) continue;
                sum += item.Quantity;
            }
        }
        return sum;
    }

    public bool CanPurchaseShopProduct(ShopType shopType, int productId)
    {
        var product = GameData.Instance.GetShopProductData(productId);
        var itemId = product.ProductItemId;

        var purchasedToday = GetPurchasedCountToday(shopType, itemId);
        if (product.RefreshAmount - purchasedToday <= 0) return false;

        if (product.MaxPurchasableQuantity > 0)
        {
            var purchasedTotal = GetPurchasedCountTotal(shopType, itemId);
            if (product.MaxPurchasableQuantity - purchasedTotal <= 0) return false;
        }

        if (GetItemQuantity(product.PriceItemId) < product.PriceQuantity) return false;

        return true;
    }

    public bool PurchaseShopProduct(ShopType shopType, int productId)
    {
        if (!CanPurchaseShopProduct(shopType, productId)) return false;

        var product = GameData.Instance.GetShopProductData(productId);
        var itemId = product.ProductItemId;

        RemoveItem(product.PriceItemId, product.PriceQuantity);
        AddReward(itemId, 1);
        RecordShopPurchase(shopType, itemId, 1);
        return true;
    }

    public void RecordShopPurchase(ShopType shopType, int itemId, int quantity)
    {
        var today = _userDataInfo.CurrentDay;
        if (!_userDataInfo.ShopPurchaseRecords.TryGetValue(today, out var shopDict))
        {
            shopDict = new Dictionary<ShopType, List<ItemInfo>>();
            _userDataInfo.ShopPurchaseRecords[today] = shopDict;
        }
        if (!shopDict.TryGetValue(shopType, out var items))
        {
            items = new List<ItemInfo>();
            shopDict[shopType] = items;
        }
        foreach (var item in items)
        {
            if (item.ItemId != itemId) continue;
            item.Quantity += quantity;
            return;
        }
        items.Add(new ItemInfo { ItemId = itemId, Quantity = quantity });
    }
}
