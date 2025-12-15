using System;
using System.Collections.Generic;
using Generated;

public partial class GameData
{
    private void LoadCutsceneData(List<object[]> rows)
    {
        if (rows.IsNullOrEmpty()) return;
        foreach (var row in rows)
        {
            var newData = new CutsceneData(Convert.ToInt32(row[0]), Convert.ToInt32(row[1]), Convert.ToInt32(row[2]), (CutsceneCommand)Enum.Parse(typeof(CutsceneCommand), (string)row[3], true), (row[4] as string) ?? string.Empty, (row[5] as string).ParseIntList(), (row[6] as string).ParseStringList(), (row[7] as string).ParseVector2());
            _dtCutsceneData.Add(newData);
        }
    }

    private void LoadCutsceneGroupData(List<object[]> rows)
    {
        if (rows.IsNullOrEmpty()) return;
        foreach (var row in rows)
        {
            var newData = new CutsceneGroupData(Convert.ToInt32(row[0]), (row[1] as string) ?? string.Empty, (RequirementType)Enum.Parse(typeof(RequirementType), (string)row[2], true), (row[3] as string).ParseIntList(), Convert.ToInt32(row[4]), (row[5] as string).ParseIntList(), (row[6] as string).ParseIntList(), Convert.ToBoolean(row[7]));
            _dtCutsceneGroupData.Add(newData.CutsceneGroupId, newData);
        }
    }

    private void LoadEnumData(List<object[]> rows)
    {
        if (rows.IsNullOrEmpty()) return;
        foreach (var row in rows)
        {
            var newData = new EnumData((row[0] as string) ?? string.Empty, (row[1] as string) ?? string.Empty, (row[2] as string) ?? string.Empty, (row[3] as string) ?? string.Empty, (row[4] as string) ?? string.Empty);
            _dtEnumData.Add(newData);
        }
    }

    private void LoadGameSettingData(List<object[]> rows)
    {
        if (rows.IsNullOrEmpty()) return;
        foreach (var row in rows)
        {
            var newData = new GameSettingData((row[0] as string) ?? string.Empty, (row[1] as string) ?? string.Empty, (row[2] as string) ?? string.Empty, (row[3] as string) ?? string.Empty);
            _dtGameSettingData.Add(newData);
        }
    }

    private void LoadItemData(List<object[]> rows)
    {
        if (rows.IsNullOrEmpty()) return;
        foreach (var row in rows)
        {
            var newData = new ItemData(Convert.ToInt32(row[0]), (row[1] as string) ?? string.Empty, (row[2] as string) ?? string.Empty);
            _dtItemData.Add(newData.Id, newData);
        }
    }

    private void LoadMapNpcInfoData(List<object[]> rows)
    {
        if (rows.IsNullOrEmpty()) return;
        foreach (var row in rows)
        {
            var newData = new MapNpcInfoData(Convert.ToInt32(row[0]), (RequirementType)Enum.Parse(typeof(RequirementType), (string)row[1], true), (row[2] as string).ParseIntList(), (RequirementType)Enum.Parse(typeof(RequirementType), (string)row[3], true), (row[4] as string).ParseIntList());
            _dtMapNpcInfoData.Add(newData.NpcId, newData);
        }
    }

    private void LoadMapNpcMenuData(List<object[]> rows)
    {
        if (rows.IsNullOrEmpty()) return;
        foreach (var row in rows)
        {
            var newData = new MapNpcMenuData(Convert.ToInt32(row[0]), (RequirementType)Enum.Parse(typeof(RequirementType), (string)row[1], true), (row[2] as string).ParseIntList(), (RequirementType)Enum.Parse(typeof(RequirementType), (string)row[3], true), (row[4] as string).ParseIntList(), Convert.ToInt32(row[5]), (NpcMenuFunctionType)Enum.Parse(typeof(NpcMenuFunctionType), (string)row[6], true), Convert.ToInt32(row[7]), (row[8] as string) ?? string.Empty);
            _dtMapNpcMenuData.Add(newData);
        }
    }

    private void LoadRequirementInfoData(List<object[]> rows)
    {
        if (rows.IsNullOrEmpty()) return;
        foreach (var row in rows)
        {
            var newData = new RequirementInfoData((RequirementType)Enum.Parse(typeof(RequirementType), (string)row[0], true));
            _dtRequirementInfoData.Add(newData.RequirementType, newData);
        }
    }

    private void InvokeLoadForSheet(string sheetName, List<object[]> rows)
    {
        switch (sheetName)
        {
            case "Cutscene": LoadCutsceneData(rows); break;
            case "CutsceneGroup": LoadCutsceneGroupData(rows); break;
            case "Item": LoadItemData(rows); break;
            case "MapNpcInfo": LoadMapNpcInfoData(rows); break;
            case "MapNpcMenu": LoadMapNpcMenuData(rows); break;
            case "RequirementInfo": LoadRequirementInfoData(rows); break;
        }
    }
}
