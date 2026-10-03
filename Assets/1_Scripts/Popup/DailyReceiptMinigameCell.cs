using JetBrains.Annotations;
using Lunaria;
using UnityEngine;

public class DailyReceiptMinigameCell : MonoBehaviour
{
    [SerializeField, CanBeNull] private Image _shopImage;
    [SerializeField] private Image _itemImage;
    [SerializeField] private Text _quantityText;
    [SerializeField] private Image _familiarImage;

    private const int FamiliarImageFrameNumber = 1;
    private const string SlimeShopIconResourceKey = "slime_03"; // TODO(지선): 슬라임 미니게임용 가게 이미지 정해지면 교체

    public void SetPlayerRewardData(MinigameType minigameType, int itemId, long quantity)
    {
        SetRewardData(minigameType, itemId, quantity);
        _familiarImage.SetActive(false);
    }

    public void SetFamiliarRewardData(MinigameType minigameType, int familiarCallItemId, int itemId, long quantity)
    {
        SetRewardData(minigameType, itemId, quantity);

        var familiarCallData = GameData.Instance.GetFamiliarCallData(familiarCallItemId);
        _familiarImage.SetActive(true);
        _familiarImage.SetSprite(ResourceManager.Instance.LoadFamiliarSprite(familiarCallData.ResourceKey, true, FamiliarImageFrameNumber));
    }

    private void SetRewardData(MinigameType minigameType, int itemId, long quantity)
    {
        if (_shopImage != null)
        {
            _shopImage.SetSprite(ResourceManager.Instance.LoadSprite(GetShopIconResourceKey(minigameType)));
        }

        var itemData = GameData.Instance.GetItemData(itemId);
        _itemImage.SetSprite(ResourceManager.Instance.LoadSprite(itemData.IconResourceKey));
        _quantityText.SetText(LocalizationKey.ItemCount1.Format(quantity));
    }

    private static string GetShopIconResourceKey(MinigameType minigameType)
    {
        // 슬라임 미니게임은 가게가 없어서 ShopInfo에 아이콘이 없다.
        if (minigameType is MinigameType.Slime) return SlimeShopIconResourceKey;

        GameData.Instance.TryGetShopInfoDataByMinigameType(minigameType, out var shopInfoData);
        return shopInfoData.IconResourceKey;
    }
}