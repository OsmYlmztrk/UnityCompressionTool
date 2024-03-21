using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor; 
using UnityEngine; 
using Noss.Utilities;

[InitializeOnLoad]
public class TextureCompressEditor
{
    private int total = 0;
    private int effectedTextures = 0;

    private static TextureCompressEditor instance;

    public static TextureCompressEditor Instance
    {
        get
        {
            if (instance == null)
                instance = new TextureCompressEditor();

            return instance;
        }
    }

    public List<Texture2D> GetTextures(string[] searchInFolders)
    {
        string[] guids;
        List<Texture2D> texturesList = new List<Texture2D>();

        guids = AssetDatabase.FindAssets("t:" + typeof(Texture2D).Name, searchInFolders);

        foreach (string guid in guids)
        {
            texturesList.Add(AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guid)));
        }

        return texturesList;
    }

    public List<string> GetTexturesPath(string[] searchInFolders)
    {
        string[] guids;
        List<string> texturePathList = new List<string>();

        guids = AssetDatabase.FindAssets("t:" + typeof(Texture2D).Name, searchInFolders);

        foreach (string guid in guids)
        {
            texturePathList.Add(AssetDatabase.GUIDToAssetPath(guid));
        }

        return texturePathList;
    }

    public void ChangeTextureSettings()
    {
        MyEditorCoroutines.Execute(Delay(), OnUpdate);
      
        IEnumerator Delay()
        {
            var startTime = EditorApplication.timeSinceStartup;
            string[] searchInFolders = { "Assets/Textures" };
            List<string> textures = Instance.GetTexturesPath(searchInFolders);
            effectedTextures = 0;
            TextureSettings textureSettings = new TextureSettings(64, TextureImporterFormat.ARGB16, TextureImporterCompression.CompressedLQ, 25, true, true);

            total = textures.Count;
           
            foreach (var texturePath in textures)
            {
                TextureImporter texture = (TextureImporter)TextureImporter.GetAtPath(texturePath);

                texture.maxTextureSize = textureSettings.maxTextureSize;
                texture.textureFormat = textureSettings.textureFormat;
                texture.textureCompression = textureSettings.textureCompression;
                texture.compressionQuality = textureSettings.compressionQuality;
                texture.crunchedCompression = textureSettings.crunchedCompression;
                texture.mipmapEnabled = textureSettings.mipmapEnabled;
                EditorUtility.SetDirty(texture);
                //texture.SaveAndReimport();
                
                effectedTextures++;
                yield return new WaitForEndOfFrame();
            }

            var finishTime = EditorApplication.timeSinceStartup;

            Debug.Log("Effected Textures:" + effectedTextures + " Time:" + (finishTime - startTime));
        }
    }

    private void OnUpdate(bool t)
    {
        if (!t)
        {
            EditorUtility.DisplayProgressBar("Simple Progress Bar", "Doing some work...", effectedTextures / total);
        }
        else
        {
            EditorUtility.ClearProgressBar();
        }
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

}

