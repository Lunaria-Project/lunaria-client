using System;
using System.Collections.Generic;
using Generated;

public partial class GameData
{
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
            var newData = new MapNpcMenuData(Convert.ToInt32(row[0]), (row[1] as string) ?? string.Empty, (row[2] as string).ParseIntList(), (row[3] as string) ?? string.Empty, (row[4] as string).ParseIntList(), Convert.ToInt32(row[5]), (row[6] as string) ?? string.Empty);
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
            case "ItemData": LoadItemData(rows); break;
            case "MapNpcInfoData": LoadMapNpcInfoData(rows); break;
            case "MapNpcMenuData": LoadMapNpcMenuData(rows); break;
            case "RequirementInfoData": LoadRequirementInfoData(rows); break;
        }
    }
}
