using UnityEditor.Presets;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;
using System;

public class EditTexture : MonoBehaviour
{
    //public ImporterCopy importerCopy;
    //public static TextureSettings textureSettings;

    [MenuItem("TextureCompressor/Execute")]
    public static void Execute()
    {
        TextureCompressEditor.Instance.ChangeTextureSettings();
    }
    /*
  static void ChangeTextureSettings()
  {
      string[] searchInFolders = { "Assets/Textures/MyTextures" };
      var textures = TextureCompressEditor.Instance.GetTexturesPath(searchInFolders);
      int effectedTextures = 0;

      textureSettings = new TextureSettings(64, TextureImporterFormat.ARGB16, TextureImporterCompression.CompressedLQ, 25, true, true);
      var startTime = EditorApplication.timeSinceStartup;

      foreach (var texturePath in textures)
      {
          TextureImporter texture = (TextureImporter)TextureImporter.GetAtPath(texturePath);

          texture.maxTextureSize = textureSettings.maxTextureSize;
          texture.textureFormat = textureSettings.textureFormat;
          texture.textureCompression = textureSettings.textureCompression;
          texture.compressionQuality = textureSettings.compressionQuality;
          texture.crunchedCompression = textureSettings.crunchedCompression;
          texture.mipmapEnabled = textureSettings.mipmapEnabled;
          effectedTextures++;
          EditorUtility.SetDirty(texture);
          texture.SaveAndReimport();
      }
      var finishTime = EditorApplication.timeSinceStartup;

      Debug.Log("Effected Textures:" + effectedTextures + " Time:" + (finishTime - startTime));
  }

  [Serializable]
  public class TextureSettings
  {
      public int maxTextureSize;
      public TextureImporterFormat textureFormat;
      public TextureImporterCompression textureCompression;
      [Range(0, 100)] public int compressionQuality;
      public bool crunchedCompression;
      public bool mipmapEnabled;

      public TextureSettings(
          int maxTextureSize,
          TextureImporterFormat textureFormat,
          TextureImporterCompression textureCompression,
          int compressionQuality,
          bool crunchedCompression,
          bool mipmapEnabled)
      {
          this.maxTextureSize = maxTextureSize;
          this.textureFormat = textureFormat;
          this.textureCompression = textureCompression;
          this.compressionQuality = compressionQuality;
          this.crunchedCompression = crunchedCompression;
          this.mipmapEnabled = mipmapEnabled;
      }
  }
  */
}