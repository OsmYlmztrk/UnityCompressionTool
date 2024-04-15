using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Soda.Utilities;

namespace Soda
{
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

        public void ClearSlectedOverride(string[] searchInFolders, int overrideOption)
        {
            List<string> textures = Instance.GetTexturesPath(searchInFolders);
            string platform = "Standalone";
            switch (overrideOption)
            {
                case 0:
                    platform = "Default";
                    break;
                case 1:
                    platform = "Standalone";
                    break;
                case 2:
                    platform = "Android";
                    break;
                case 3:
                    platform = "iOS";
                    break;
            }
            foreach (var texturePath in textures)
            {
                TextureImporter texture = (TextureImporter)TextureImporter.GetAtPath(texturePath);
                texture.ClearPlatformTextureSettings(platform);
                EditorUtility.SetDirty(texture);
                texture.SaveAndReimport();
            }
        }

        public void ChangeTextureSettings(
            TextureSettings textureSettings,
            string[] searchInFolders,
            BuildTarget buildTarget,
            string platform,
            IncludePropertiesList_Texture includedPropertiesList)
        {
            MyEditorCoroutines.Execute(Delay(), OnUpdate);

            IEnumerator Delay()
            {
                var startTime = EditorApplication.timeSinceStartup;
                List<string> textures = Instance.GetTexturesPath(searchInFolders);

                effectedTextures = 0;

                total = textures.Count;
                foreach (var texturePath in textures)
                {
                    TextureImporter texture = (TextureImporter)TextureImporter.GetAtPath(texturePath);

                    if (platform != "Default")
                    {
                        TextureImporterPlatformSettings texset = texture.GetPlatformTextureSettings(platform);
                        texset.overridden = true;
                        if (includedPropertiesList.isFormat)
                        {
                            if (platform != "Default")
                            {

                                texset = texture.GetPlatformTextureSettings(platform);
                                texset.overridden = true;

                                if (TextureImporter.IsPlatformTextureFormatValid(texture.textureType, buildTarget, textureSettings.textureFormat))
                                {
                                    texset.format = textureSettings.textureFormat;
                                }
                            }
                            else
                            {

                                texset = texture.GetDefaultPlatformTextureSettings();

                                if (TextureImporter.IsDefaultPlatformTextureFormatValid(texture.textureType, textureSettings.textureFormat))
                                {
                                    texset.format = textureSettings.textureFormat;
                                }
                            }
                        }

                        if (includedPropertiesList.isTextureSize)
                            texset.maxTextureSize = textureSettings.maxTextureSize;

                        if (includedPropertiesList.isResizeAlgorithm)
                            texset.resizeAlgorithm = textureSettings.resizeAlgorithm;

                        if (includedPropertiesList.isTextureCompression)
                            texset.textureCompression = textureSettings.textureCompression;

                        if (includedPropertiesList.iscrunchedCompression)
                            texset.crunchedCompression = textureSettings.crunchedCompression;

                        if (includedPropertiesList.isCompressQuality)
                            texset.compressionQuality = textureSettings.compressionQuality;

                        texture.SetPlatformTextureSettings(texset);
                    }
                    else
                    {
                        TextureImporterPlatformSettings texset = texture.GetDefaultPlatformTextureSettings();

                        if (includedPropertiesList.isFormat)
                        {
                            if (platform != "Default")
                            {

                                texset = texture.GetPlatformTextureSettings(platform);
                                texset.overridden = true;

                                if (TextureImporter.IsPlatformTextureFormatValid(texture.textureType, buildTarget, textureSettings.textureFormat))
                                {
                                    texset.format = textureSettings.textureFormat;
                                }
                            }
                            else
                            {

                                texset = texture.GetDefaultPlatformTextureSettings();

                                if (TextureImporter.IsDefaultPlatformTextureFormatValid(texture.textureType, textureSettings.textureFormat))
                                {
                                    texset.format = textureSettings.textureFormat;
                                }
                            }
                        }

                        if (includedPropertiesList.isTextureSize)
                            texset.maxTextureSize = textureSettings.maxTextureSize;

                        if (includedPropertiesList.isResizeAlgorithm)
                            texset.resizeAlgorithm = textureSettings.resizeAlgorithm;

                        if (includedPropertiesList.isTextureCompression)
                            texset.textureCompression = textureSettings.textureCompression;

                        if (includedPropertiesList.iscrunchedCompression)
                            texset.crunchedCompression = textureSettings.crunchedCompression;

                        if (includedPropertiesList.isCompressQuality)
                            texset.compressionQuality = textureSettings.compressionQuality;

                        texture.SetPlatformTextureSettings(texset);
                        
                    }

                    EditorUtility.SetDirty(texture);
                    texture.SaveAndReimport();

                    effectedTextures++;
                    yield return new WaitForEndOfFrame();
                }

                var finishTime = EditorApplication.timeSinceStartup;

                Debug.Log("<color=Orange>Effected Textures:" + effectedTextures + " Time:" + (finishTime - startTime)+ "</color>");
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

            if (string.IsNullOrEmpty(searchInFolders[0]))
            {
                searchInFolders[0] = "Assets/";
            }

            guids = AssetDatabase.FindAssets("t:" + typeof(Texture2D).Name, searchInFolders);
            //SodaCreativeHeader
            foreach (string guid in guids)
            {
                texturePathList.Add(AssetDatabase.GUIDToAssetPath(guid));
            }
            texturePathList.RemoveAll(p => p.Contains("SodaCreativeHeader"));

            return texturePathList;
        }

        private void OnUpdate(bool t)
        {
            if (!t)
            {
                EditorUtility.DisplayProgressBar("Textures Compress", "Compression is in progress...", effectedTextures / total);
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
            public TextureResizeAlgorithm resizeAlgorithm;
            public TextureImporterFormat textureFormat;
            public TextureImporterCompression textureCompression;
            [Range(0, 100)] public int compressionQuality;
            public bool crunchedCompression;

            public TextureSettings(
                int maxTextureSize,
                TextureResizeAlgorithm resizeAlgorithm,
                TextureImporterFormat textureFormat,
                TextureImporterCompression textureCompression,
                int compressionQuality,
                bool crunchedCompression)
            {
                this.maxTextureSize = maxTextureSize;
                this.resizeAlgorithm = resizeAlgorithm;
                this.textureFormat = textureFormat;
                this.textureCompression = textureCompression;
                this.compressionQuality = compressionQuality;
                this.crunchedCompression = crunchedCompression;
            }
        }

        [Serializable]
        public class IncludePropertiesList_Texture
        {
            public bool isTextureSize;
            public bool isResizeAlgorithm;
            public bool isFormat;
            public bool isTextureCompression;
            public bool isCompressQuality;
            public bool iscrunchedCompression;

            public IncludePropertiesList_Texture(bool isTextureSize, bool isResizeAlgorithm, bool isFormat, bool isTextureCompression, bool isCompressQuality, bool iscrunchedCompression)
            {
                this.isTextureSize = isTextureSize;
                this.isResizeAlgorithm = isResizeAlgorithm;
                this.isFormat = isFormat;
                this.isTextureCompression = isTextureCompression;
                this.isCompressQuality = isCompressQuality;
                this.iscrunchedCompression = iscrunchedCompression;
            }
        }


    }

}