using UnityEngine;
using UnityEditor; 

internal sealed class CustomAssetImporter : AssetPostprocessor
{
    
    private void OnPreprocessTexture()
    {
        var importer = assetImporter as TextureImporter;
        
        importer.textureType = TextureImporterType.Sprite;
        importer.isReadable = false;
        importer.filterMode = FilterMode.Point;
        importer.spritePixelsPerUnit = 16;
    }
    private void OnPostprocessTexture(Texture2D import) { }
    
}