using UnityEditor;

public class SpritePostprocessor : AssetPostprocessor
{
    private const float DefaultPixelsPerUnit = 1f;
    private const string AppliedFlag = "SpritePostprocessor_Applied";

    private void OnPreprocessTexture()
    {
        if (!assetPath.Contains("Assets/")) return;

        var importer = (TextureImporter)assetImporter;
        if (importer.userData.Contains(AppliedFlag)) return;

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = DefaultPixelsPerUnit;
        importer.userData = AppliedFlag;
    }
}