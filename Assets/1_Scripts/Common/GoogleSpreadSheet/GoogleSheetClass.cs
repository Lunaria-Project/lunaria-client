using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
public class OAuthToken
{
    [JsonProperty("access_token")] public string AccessToken { get; set; }
    [JsonProperty("refresh_token")] public string RefreshToken { get; set; }
    [JsonProperty("token_type")] public string TokenType { get; set; }
    [JsonProperty("expires_in")] public int ExpiresIn { get; set; }
    
    public long NextRefreshTime;
}

[Serializable]
public class GoogleSheetInfoResponse
{
    [JsonProperty("sheets")] public List<SheetEntry> Sheets { get; set; }
}

[Serializable]
public class SheetEntry
{
    [JsonProperty("properties")] public SheetProperties Properties { get; set; }
}

[Serializable]
public class SheetProperties
{
    [JsonProperty("sheetId")] public int SheetId { get; set; }
    [JsonProperty("title")] public string SheetName { get; set; }
    [JsonProperty("index")] public int Index { get; set; }
    [JsonProperty("sheetType")] public string SheetType { get; set; }
    [JsonProperty("gridProperties")] public GridProperties GridProperties { get; set; }
}

[Serializable]
public class GridProperties
{
    [JsonProperty("rowCount")] public int RowCount { get; set; }
    [JsonProperty("columnCount")] public int ColumnCount { get; set; }
}

public class SheetValueResponse
{
    [JsonProperty("values")] public List<List<string>> Values { get; set; }
}

public struct SheetInfo
{
    public string SheetName { get; internal set; }
    public string[] ColumnTypes { get; internal set; }
    public string[] ColumnNames { get; internal set; }
    public Dictionary<int, List<string>> EnumList { get; internal set; }
}