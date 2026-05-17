using System;
using System.Collections.Generic;
using Generated;

public partial class GameData
{
    private void LoadCharacterInfoData(List<object[]> rows)
    {
        if (rows.IsNullOrEmpty()) return;
        _dtCharacterInfoData.Clear();
        foreach (var row in rows)
        {
            var newData = new CharacterInfoData(Convert.ToInt32(row[0]), GetLocalString((row[1] as string) ?? string.Empty), (row[2] as string) ?? string.Empty);
            _dtCharacterInfoData.Add(newData.Id, newData);
        }
    }

    private void LoadCutsceneData(List<object[]> rows)
    {
        if (rows.IsNullOrEmpty()) return;
        _dtCutsceneData.Clear();
        foreach (var row in rows)
        {
            var newData = new CutsceneData(Convert.ToInt32(row[0]), Convert.ToInt32(row[1]), ((string)row[2]).ParseEnum<CutsceneCommand>(), GetLocalString((row[3] as string) ?? string.Empty), (row[4] as string).ParseIntList(), (row[5] as string).ParseStringList(), (row[6] as string).ParseVector2());
            _dtCutsceneData.Add(newData);
        }
    }

    private void LoadCutsceneInfoData(List<object[]> rows)
    {
        if (rows.IsNullOrEmpty()) return;
        _dtCutsceneInfoData.Clear();
        foreach (var row in rows)
        {
            var newData = new CutsceneInfoData(Convert.ToInt32(row[0]), GetLocalString((row[1] as string) ?? string.Empty), ((string)row[2]).ParseEnum<RequirementType>(), (row[3] as string).ParseIntList(), Convert.ToInt32(row[4]), (row[5] as string).ParseIntList(), (row[6] as string).ParseIntList(), ((row[7] as string) ?? string.Empty).ParseBool());
            _dtCutsceneInfoData.Add(newData.CutsceneId, newData);
        }
    }

    private void LoadCutsceneSelectionData(List<object[]> rows)
    {
        if (rows.IsNullOrEmpty()) return;
        _dtCutsceneSelectionData.Clear();
        foreach (var row in rows)
        {
            var newData = new CutsceneSelectionData(Convert.ToInt32(row[0]), ((string)row[1]).ParseEnum<RequirementType>(), (row[2] as string).ParseIntList(), GetLocalString((row[3] as string) ?? string.Empty), Convert.ToInt32(row[4]));
            _dtCutsceneSelectionData.Add(newData.SelectionId, newData);
        }
    }

    private void LoadLocalizationData(List<object[]> rows)
    {
        if (rows.IsNullOrEmpty()) return;
        _dtLocalizationData.Clear();
        foreach (var row in rows)
        {
            var newData = new LocalizationData((row[0] as string) ?? string.Empty, (row[1] as string) ?? string.Empty, (row[2] as string) ?? string.Empty, (row[3] as string) ?? string.Empty, ((row[4] as string) ?? string.Empty).ParseBool());
            _dtLocalizationData.Add(newData);
        }
    }

    private void LoadArtifactData(List<object[]> rows)
    {
        if (rows.IsNullOrEmpty()) return;
        _dtArtifactData.Clear();
        foreach (var row in rows)
        {
            var newData = new ArtifactData(Convert.ToInt32(row[0]), ((string)row[1]).ParseEnum<ArtifactType>());
            _dtArtifactData.Add(newData.Id, newData);
        }
    }

    private void LoadInitialItemData(List<object[]> rows)
    {
        if (rows.IsNullOrEmpty()) return;
        _dtInitialItemData.Clear();
        foreach (var row in rows)
        {
            var newData = new InitialItemData(Convert.ToInt32(row[0]), Convert.ToInt32(row[1]));
            _dtInitialItemData.Add(newData.ItemId, newData);
        }
    }

    private void LoadInventoryTabData(List<object[]> rows)
    {
        if (rows.IsNullOrEmpty()) return;
        _dtInventoryTabData.Clear();
        foreach (var row in rows)
        {
            var newData = new InventoryTabData(((string)row[0]).ParseEnum<ItemType>(), ((string)row[1]).ParseEnum<InventoryTabType>());
            _dtInventoryTabData.Add(newData.ItemType, newData);
        }
    }

    private void LoadItemData(List<object[]> rows)
    {
        if (rows.IsNullOrEmpty()) return;
        _dtItemData.Clear();
        foreach (var row in rows)
        {
            var newData = new ItemData(Convert.ToInt32(row[0]), GetLocalString((row[1] as string) ?? string.Empty), GetLocalString((row[2] as string) ?? string.Empty), (row[3] as string) ?? string.Empty, ((string)row[4]).ParseEnum<ItemType>(), Convert.ToInt32(row[5]));
            _dtItemData.Add(newData.Id, newData);
        }
    }

    private void LoadLoadingData(List<object[]> rows)
    {
        if (rows.IsNullOrEmpty()) return;
        _dtLoadingData.Clear();
        foreach (var row in rows)
        {
            var newData = new LoadingData(Convert.ToInt32(row[0]), GetLocalString((row[1] as string) ?? string.Empty), (row[2] as string) ?? string.Empty, ((string)row[3]).ParseEnum<LoadingType>());
            _dtLoadingData.Add(newData.Id, newData);
        }
    }

    private void LoadMapNpcInfoData(List<object[]> rows)
    {
        if (rows.IsNullOrEmpty()) return;
        _dtMapNpcInfoData.Clear();
        foreach (var row in rows)
        {
            var newData = new MapNpcInfoData(Convert.ToInt32(row[0]), Convert.ToInt32(row[1]), (row[2] as string).ParseFloatList(), Convert.ToSingle(row[3]), (row[4] as string).ParseFloatList());
            _dtMapNpcInfoData.Add(newData.NpcId, newData);
        }
    }

    private void LoadMapNpcMenuData(List<object[]> rows)
    {
        if (rows.IsNullOrEmpty()) return;
        _dtMapNpcMenuData.Clear();
        foreach (var row in rows)
        {
            var newData = new MapNpcMenuData(Convert.ToInt32(row[0]), GetLocalString((row[1] as string) ?? string.Empty), ((string)row[2]).ParseEnum<RequirementType>(), (row[3] as string).ParseIntList(), Convert.ToInt32(row[4]), Convert.ToInt32(row[5]), ((row[6] as string) ?? string.Empty).ParseBool(), ((string)row[7]).ParseEnum<NpcMenuFunctionType>(), Convert.ToInt32(row[8]));
            _dtMapNpcMenuData.Add(newData);
        }
    }

    private void LoadMapNpcPositionData(List<object[]> rows)
    {
        if (rows.IsNullOrEmpty()) return;
        _dtMapNpcPositionData.Clear();
        foreach (var row in rows)
        {
            var newData = new MapNpcPositionData(Convert.ToInt32(row[0]), ((string)row[1]).ParseEnum<RequirementType>(), (row[2] as string).ParseIntList(), ((string)row[3]).ParseEnum<MapType>(), (row[4] as string).ParseFloatList());
            _dtMapNpcPositionData.Add(newData);
        }
    }

    private void LoadMapStaticNpcMenuData(List<object[]> rows)
    {
        if (rows.IsNullOrEmpty()) return;
        _dtMapStaticNpcMenuData.Clear();
        foreach (var row in rows)
        {
            var newData = new MapStaticNpcMenuData(Convert.ToInt32(row[0]), GetLocalString((row[1] as string) ?? string.Empty), ((string)row[2]).ParseEnum<RequirementType>(), (row[3] as string).ParseIntList(), Convert.ToInt32(row[4]), Convert.ToInt32(row[5]), ((string)row[6]).ParseEnum<NpcMenuFunctionType>(), Convert.ToInt32(row[7]));
            _dtMapStaticNpcMenuData.Add(newData);
        }
    }

    private void LoadMinigameInfoData(List<object[]> rows)
    {
        if (rows.IsNullOrEmpty()) return;
        _dtMinigameInfoData.Clear();
        foreach (var row in rows)
        {
            var newData = new MinigameInfoData(((string)row[0]).ParseEnum<MinigameType>(), Convert.ToInt32(row[1]), Convert.ToInt32(row[2]));
            _dtMinigameInfoData.Add(newData.MinigameType, newData);
        }
    }

    private void LoadMinigameRewardData(List<object[]> rows)
    {
        if (rows.IsNullOrEmpty()) return;
        _dtMinigameRewardData.Clear();
        foreach (var row in rows)
        {
            var newData = new MinigameRewardData(((string)row[0]).ParseEnum<MinigameType>(), Convert.ToInt32(row[1]), (row[2] as string).ParseIntList(), (row[3] as string).ParseIntList());
            _dtMinigameRewardData.Add(newData);
        }
    }

    private void LoadRequirementInfoData(List<object[]> rows)
    {
        if (rows.IsNullOrEmpty()) return;
        _dtRequirementInfoData.Clear();
        foreach (var row in rows)
        {
            var newData = new RequirementInfoData(((string)row[0]).ParseEnum<RequirementType>());
            _dtRequirementInfoData.Add(newData.RequirementType, newData);
        }
    }

    private void LoadShopInfoData(List<object[]> rows)
    {
        if (rows.IsNullOrEmpty()) return;
        _dtShopInfoData.Clear();
        foreach (var row in rows)
        {
            var newData = new ShopInfoData(Convert.ToInt32(row[0]), Convert.ToInt32(row[1]), Convert.ToInt32(row[2]), ((string)row[3]).ParseEnum<ShopType>(), ((string)row[4]).ParseEnum<MinigameType>());
            _dtShopInfoData.Add(newData.ShopId, newData);
        }
    }

    private void LoadShopProductData(List<object[]> rows)
    {
        if (rows.IsNullOrEmpty()) return;
        _dtShopProductData.Clear();
        foreach (var row in rows)
        {
            var newData = new ShopProductData(Convert.ToInt32(row[0]), Convert.ToInt32(row[1]), Convert.ToInt32(row[2]), Convert.ToInt32(row[3]), Convert.ToInt32(row[4]), ((string)row[5]).ParseEnum<RequirementType>(), (row[6] as string).ParseIntList(), Convert.ToInt32(row[7]), Convert.ToInt32(row[8]));
            _dtShopProductData.Add(newData.ProductId, newData);
        }
    }

    private void InvokeLoadForSheet(string sheetName, List<object[]> rows)
    {
        switch (sheetName)
        {
            case "CharacterInfo": LoadCharacterInfoData(rows); break;
            case "Cutscene": LoadCutsceneData(rows); break;
            case "CutsceneInfo": LoadCutsceneInfoData(rows); break;
            case "CutsceneSelection": LoadCutsceneSelectionData(rows); break;
            case "Localization": LoadLocalizationData(rows); break;
            case "Artifact": LoadArtifactData(rows); break;
            case "InitialItem": LoadInitialItemData(rows); break;
            case "InventoryTab": LoadInventoryTabData(rows); break;
            case "Item": LoadItemData(rows); break;
            case "Loading": LoadLoadingData(rows); break;
            case "MapNpcInfo": LoadMapNpcInfoData(rows); break;
            case "MapNpcMenu": LoadMapNpcMenuData(rows); break;
            case "MapNpcPosition": LoadMapNpcPositionData(rows); break;
            case "MapStaticNpcMenu": LoadMapStaticNpcMenuData(rows); break;
            case "MinigameInfo": LoadMinigameInfoData(rows); break;
            case "MinigameReward": LoadMinigameRewardData(rows); break;
            case "RequirementInfo": LoadRequirementInfoData(rows); break;
            case "ShopInfo": LoadShopInfoData(rows); break;
            case "ShopProduct": LoadShopProductData(rows); break;
        }
    }
}
