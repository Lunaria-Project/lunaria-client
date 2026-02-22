using System;
using System.Collections.Generic;
using Generated;

public partial class GameData
{
    private void LoadCharacterInfoData(List<object[]> rows, LocalType type)
    {
        if (rows.IsNullOrEmpty()) return;
        _dtCharacterInfoData.Clear();
        foreach (var row in rows)
        {
            var newData = new CharacterInfoData(Convert.ToInt32(row[0]), GetLocalString((row[1] as string) ?? string.Empty, type), (row[2] as string) ?? string.Empty);
            _dtCharacterInfoData.Add(newData.Id, newData);
        }
    }

    private void LoadCutsceneData(List<object[]> rows, LocalType type)
    {
        if (rows.IsNullOrEmpty()) return;
        _dtCutsceneData.Clear();
        foreach (var row in rows)
        {
            var newData = new CutsceneData(Convert.ToInt32(row[0]), Convert.ToInt32(row[1]), (CutsceneCommand)Enum.Parse(typeof(CutsceneCommand), (string)row[2], true), GetLocalString((row[3] as string) ?? string.Empty, type), (row[4] as string).ParseIntList(), (row[5] as string).ParseStringList(), (row[6] as string).ParseVector2());
            _dtCutsceneData.Add(newData);
        }
    }

    private void LoadCutsceneInfoData(List<object[]> rows, LocalType type)
    {
        if (rows.IsNullOrEmpty()) return;
        _dtCutsceneInfoData.Clear();
        foreach (var row in rows)
        {
            var newData = new CutsceneInfoData(Convert.ToInt32(row[0]), GetLocalString((row[1] as string) ?? string.Empty, type), (RequirementType)Enum.Parse(typeof(RequirementType), (string)row[2], true), (row[3] as string).ParseIntList(), Convert.ToInt32(row[4]), (row[5] as string).ParseIntList(), (row[6] as string).ParseIntList(), Convert.ToBoolean(row[7]));
            _dtCutsceneInfoData.Add(newData.CutsceneId, newData);
        }
    }

    private void LoadCutsceneSelectionData(List<object[]> rows, LocalType type)
    {
        if (rows.IsNullOrEmpty()) return;
        _dtCutsceneSelectionData.Clear();
        foreach (var row in rows)
        {
            var newData = new CutsceneSelectionData(Convert.ToInt32(row[0]), (RequirementType)Enum.Parse(typeof(RequirementType), (string)row[1], true), (row[2] as string).ParseIntList(), GetLocalString((row[3] as string) ?? string.Empty, type), Convert.ToInt32(row[4]));
            _dtCutsceneSelectionData.Add(newData.SelectionId, newData);
        }
    }

    private void LoadArtifactData(List<object[]> rows, LocalType type)
    {
        if (rows.IsNullOrEmpty()) return;
        _dtArtifactData.Clear();
        foreach (var row in rows)
        {
            var newData = new ArtifactData(Convert.ToInt32(row[0]), (ArtifactType)Enum.Parse(typeof(ArtifactType), (string)row[1], true));
            _dtArtifactData.Add(newData.Id, newData);
        }
    }

    private void LoadInitialItemData(List<object[]> rows, LocalType type)
    {
        if (rows.IsNullOrEmpty()) return;
        _dtInitialItemData.Clear();
        foreach (var row in rows)
        {
            var newData = new InitialItemData(Convert.ToInt32(row[0]), Convert.ToInt32(row[1]));
            _dtInitialItemData.Add(newData.ItemId, newData);
        }
    }

    private void LoadItemData(List<object[]> rows, LocalType type)
    {
        if (rows.IsNullOrEmpty()) return;
        _dtItemData.Clear();
        foreach (var row in rows)
        {
            var newData = new ItemData(Convert.ToInt32(row[0]), GetLocalString((row[1] as string) ?? string.Empty, type), (row[2] as string) ?? string.Empty, (ItemType)Enum.Parse(typeof(ItemType), (string)row[3], true), Convert.ToInt32(row[4]));
            _dtItemData.Add(newData.Id, newData);
        }
    }

    private void LoadLoadingData(List<object[]> rows, LocalType type)
    {
        if (rows.IsNullOrEmpty()) return;
        _dtLoadingData.Clear();
        foreach (var row in rows)
        {
            var newData = new LoadingData(Convert.ToInt32(row[0]), GetLocalString((row[1] as string) ?? string.Empty, type), (row[2] as string) ?? string.Empty, (LoadingType)Enum.Parse(typeof(LoadingType), (string)row[3], true));
            _dtLoadingData.Add(newData.Id, newData);
        }
    }

    private void LoadMapNpcInfoData(List<object[]> rows, LocalType type)
    {
        if (rows.IsNullOrEmpty()) return;
        _dtMapNpcInfoData.Clear();
        foreach (var row in rows)
        {
            var newData = new MapNpcInfoData(Convert.ToInt32(row[0]), Convert.ToInt32(row[1]));
            _dtMapNpcInfoData.Add(newData.NpcId, newData);
        }
    }

    private void LoadMapNpcMenuData(List<object[]> rows, LocalType type)
    {
        if (rows.IsNullOrEmpty()) return;
        _dtMapNpcMenuData.Clear();
        foreach (var row in rows)
        {
            var newData = new MapNpcMenuData(Convert.ToInt32(row[0]), GetLocalString((row[1] as string) ?? string.Empty, type), (RequirementType)Enum.Parse(typeof(RequirementType), (string)row[2], true), (row[3] as string).ParseIntList(), Convert.ToInt32(row[4]), Convert.ToInt32(row[5]), Convert.ToBoolean(row[6]), (NpcMenuFunctionType)Enum.Parse(typeof(NpcMenuFunctionType), (string)row[7], true), Convert.ToInt32(row[8]));
            _dtMapNpcMenuData.Add(newData);
        }
    }

    private void LoadRequirementInfoData(List<object[]> rows, LocalType type)
    {
        if (rows.IsNullOrEmpty()) return;
        _dtRequirementInfoData.Clear();
        foreach (var row in rows)
        {
            var newData = new RequirementInfoData((RequirementType)Enum.Parse(typeof(RequirementType), (string)row[0], true));
            _dtRequirementInfoData.Add(newData.RequirementType, newData);
        }
    }

    private void LoadShopInfoData(List<object[]> rows, LocalType type)
    {
        if (rows.IsNullOrEmpty()) return;
        _dtShopInfoData.Clear();
        foreach (var row in rows)
        {
            var newData = new ShopInfoData(Convert.ToInt32(row[0]), Convert.ToInt32(row[1]), Convert.ToInt32(row[2]), (ShopType)Enum.Parse(typeof(ShopType), (string)row[3], true));
            _dtShopInfoData.Add(newData.ShopId, newData);
        }
    }

    private void InvokeLoadForSheet(string sheetName, List<object[]> rows, LocalType type)
    {
        switch (sheetName)
        {
            case "CharacterInfo": LoadCharacterInfoData(rows, type); break;
            case "Cutscene": LoadCutsceneData(rows, type); break;
            case "CutsceneInfo": LoadCutsceneInfoData(rows, type); break;
            case "CutsceneSelection": LoadCutsceneSelectionData(rows, type); break;
            case "Artifact": LoadArtifactData(rows, type); break;
            case "InitialItem": LoadInitialItemData(rows, type); break;
            case "Item": LoadItemData(rows, type); break;
            case "Loading": LoadLoadingData(rows, type); break;
            case "MapNpcInfo": LoadMapNpcInfoData(rows, type); break;
            case "MapNpcMenu": LoadMapNpcMenuData(rows, type); break;
            case "RequirementInfo": LoadRequirementInfoData(rows, type); break;
            case "ShopInfo": LoadShopInfoData(rows, type); break;
        }
    }
}
