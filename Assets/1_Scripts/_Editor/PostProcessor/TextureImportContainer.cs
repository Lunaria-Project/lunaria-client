using UnityEngine;
#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
#endif

/// <summary>
/// 폴더에 넣어두고 해당 폴더의 텍스처 임포트 설정을 일괄 적용하는 컨테이너.
/// </summary>
public class TextureImportContainer : ScriptableObject
{
    [SerializeField] private SpriteMeshType _spriteMeshType = SpriteMeshType.Tight;
    [SerializeField] private bool _isReadable;

#if UNITY_EDITOR
    private const string SettingSuffix = "_setting";
    private const string AssetExtension = ".asset";
    private const string MetaExtension = ".meta";

    public static TextureImportContainer Find(string assetPath)
    {
        var folderPath = ToAssetPath(Path.GetDirectoryName(assetPath));
        while (!string.IsNullOrEmpty(folderPath))
        {
            var container = LoadContainer(folderPath);
            if (container != null) return container;

            var lastSlashIndex = folderPath.LastIndexOf('/');
            if (lastSlashIndex <= 0) break;

            folderPath = folderPath.Substring(0, lastSlashIndex);
        }

        return null;
    }

    public void Apply(TextureImporter importer)
    {
        importer.isReadable = _isReadable;

        var settings = new TextureImporterSettings();
        importer.ReadTextureSettings(settings);
        settings.spriteMeshType = _spriteMeshType;
        importer.SetTextureSettings(settings);
    }

    [MenuItem("Assets/Create/Lunaria/texture_import_container", false, 80)]
    private static void CreateInSelectedFolder()
    {
        var folderPath = GetSelectedFolderPath();
        if (string.IsNullOrEmpty(folderPath))
        {
            LogManager.LogError("[TextureImport] Create: 폴더를 선택한 뒤 실행해주세요.");
            return;
        }

        var folderName = Path.GetFileName(folderPath);
        var assetPath = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/{folderName}{SettingSuffix}{AssetExtension}");
        var container = CreateInstance<TextureImportContainer>();

        AssetDatabase.CreateAsset(container, assetPath);
        AssetDatabase.SaveAssets();
        Selection.activeObject = container;

        LogManager.Log($"[TextureImport] Create: {assetPath}");
    }

    private static string GetSelectedFolderPath()
    {
        foreach (var assetGuid in Selection.assetGUIDs)
        {
            var path = AssetDatabase.GUIDToAssetPath(assetGuid);
            if (string.IsNullOrEmpty(path)) continue;
            if (AssetDatabase.IsValidFolder(path)) return path;

            return ToAssetPath(Path.GetDirectoryName(path));
        }

        return null;
    }

    private static void CollectFilePaths(string folderPath, List<string> filePaths)
    {
        foreach (var filePath in Directory.GetFiles(folderPath))
        {
            if (filePath.EndsWith(MetaExtension)) continue;

            filePaths.Add(ToAssetPath(filePath));
        }

        foreach (var subFolderPath in Directory.GetDirectories(folderPath))
        {
            var assetFolderPath = ToAssetPath(subFolderPath);

            // 하위 폴더가 자기 컨테이너를 갖고 있으면 그쪽 버튼에 맡긴다.
            if (LoadContainer(assetFolderPath) != null) continue;

            CollectFilePaths(assetFolderPath, filePaths);
        }
    }

    private static TextureImportContainer LoadContainer(string folderPath)
    {
        if (!Directory.Exists(folderPath)) return null;

        foreach (var filePath in Directory.GetFiles(folderPath, $"*{AssetExtension}"))
        {
            var container = AssetDatabase.LoadAssetAtPath<TextureImportContainer>(ToAssetPath(filePath));
            if (container != null) return container;
        }

        return null;
    }

    private static string ToAssetPath(string path)
    {
        return string.IsNullOrEmpty(path) ? null : path.Replace('\\', '/');
    }

    [Button("이 폴더에 적용")]
    private void ApplyToFolder()
    {
        var folderPath = ToAssetPath(Path.GetDirectoryName(AssetDatabase.GetAssetPath(this)));
        if (string.IsNullOrEmpty(folderPath))
        {
            LogManager.LogError($"[TextureImport] {name}: 컨테이너가 속한 폴더를 찾을 수 없습니다.");
            return;
        }

        var filePaths = new List<string>();
        CollectFilePaths(folderPath, filePaths);

        var appliedCount = 0;
        foreach (var filePath in filePaths)
        {
            if (AssetImporter.GetAtPath(filePath) is not TextureImporter importer) continue;

            Apply(importer);
            importer.SaveAndReimport();
            appliedCount++;
        }

        LogManager.Log($"[TextureImport] {name}: {appliedCount}개 텍스처에 적용했습니다. Folder: {folderPath}");
    }
#endif
}
