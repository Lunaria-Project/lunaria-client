using System;
using System.Collections.Generic;
using Generated;

public partial class GameData
{
    private void LoadCutscene(List<object[]> rows)
    {
        if (rows.IsNullOrEmpty()) return;
        foreach (var row in rows)
        {
            var newData = new Cutscene(Convert.ToInt32(row[0]), Convert.ToInt32(row[1]), Convert.ToInt32(row[2]), (row[3] as string) ?? string.Empty, (row[4] as string) ?? string.Empty, (row[5] as string).ParseIntList(), (row[6] as string).ParseStringList(), (row[7] as string).ParseVector2());
            _dtCutscene.Add(newData);
        }
    }

    private void LoadCutsceneGroup(List<object[]> rows)
    {
        if (rows.IsNullOrEmpty()) return;
        foreach (var row in rows)
        {
            var newData = new CutsceneGroup(Convert.ToInt32(row[0]), (row[1] as string) ?? string.Empty, (row[2] as string) ?? string.Empty, (row[3] as string).ParseIntList(), Convert.ToInt32(row[4]), (row[5] as string).ParseIntList(), (row[6] as string).ParseIntList(), Convert.ToBoolean(row[7]));
            _dtCutsceneGroup.Add(newData.CutsceneGroupId, newData);
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
            var newData = new MapNpcInfoData(Convert.ToInt32(row[0]), (row[1] as string) ?? string.Empty, (row[2] as string).ParseIntList(), (row[3] as string) ?? string.Empty, (row[4] as string).ParseIntList());
            _dtMapNpcInfoData.Add(newData.NpcId, newData);
        }
    }

    private void LoadMapNpcMenuData(List<object[]> rows)
    {
        if (rows.IsNullOrEmpty()) return;
        foreach (var row in rows)
        {
            var newData = new MapNpcMenuData(Convert.ToInt32(row[0]), (row[1] as string) ?? string.Empty, (row[2] as string).ParseIntList(), (row[3] as string) ?? string.Empty, (row[4] as string).ParseIntList(), Convert.ToInt32(row[5]), (row[6] as string) ?? string.Empty, Convert.ToInt32(row[7]), (row[8] as string) ?? string.Empty);
            _dtMapNpcMenuData.Add(newData);
        }
    }

    private void LoadRequirementInfoData(List<object[]> rows)
    {
        if (rows.IsNullOrEmpty()) return;
        foreach (var row in rows)
        {
            var newData = new RequirementInfoData((row[0] as string) ?? string.Empty);
            _dtRequirementInfoData.Add(newData.RequirementType, newData);
        }
    }

    private void InvokeLoadForSheet(string sheetName, List<object[]> rows)
    {
        switch (sheetName)
        {
            case "Cutscene": LoadCutscene(rows); break;
            case "CutsceneGroup": LoadCutsceneGroup(rows); break;
            case "ItemData": LoadItemData(rows); break;
            case "MapNpcInfoData": LoadMapNpcInfoData(rows); break;
            case "MapNpcMenuData": LoadMapNpcMenuData(rows); break;
            case "RequirementInfoData": LoadRequirementInfoData(rows); break;
        }
    }
}
