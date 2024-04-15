using Soda.Utilities;
using System.Collections;
using System.Collections.Generic; 
using UnityEditor;
using UnityEngine; 

namespace Soda
{
    [InitializeOnLoad]
    public class AudioCompressEditor
    {
     
        private int total = 0;
        private int effectedTextures = 0;

        private static AudioCompressEditor instance;

        public static AudioCompressEditor Instance
        {
            get
            {
                if (instance == null)
                    instance = new AudioCompressEditor();

                return instance;
            }
        }
        public List<string> GetAudioPath(string[] searchInFolders)
        {
             
            string[] guids;
            List<string> meshesPathList = new List<string>();

            guids = AssetDatabase.FindAssets("t:" + typeof(AudioClip).Name, searchInFolders);

            foreach (string guid in guids)
            {
                meshesPathList.Add(AssetDatabase.GUIDToAssetPath(guid));
            }

            return meshesPathList;
        }

        public void ChangeAudioSettings(bool forceToMono, bool normalize, bool preloadAudioData, AudioImporterSampleSettings audioImporterSample,
            string[] searchInFolders, string platform, IncludePropertiesList_Audio includeProperties)
        {
            var audios = GetAudioPath(searchInFolders);
            total = audios.Count;
            effectedTextures = 0;
            MyEditorCoroutines.Execute(Delay(), OnUpdate);

            IEnumerator Delay()
            {
                var startTime = EditorApplication.timeSinceStartup;
                foreach (var item in audios)
                {
                    effectedTextures++;

                    AudioImporter audioImporter = (AudioImporter)AudioImporter.GetAtPath(item);

                    if (includeProperties.isPreloadData)
                    {
                        audioImporter.preloadAudioData = preloadAudioData;
                    }

                    if (includeProperties.isForceToMono)
                    {
                        audioImporter.forceToMono = forceToMono;
                    }

                    if (includeProperties.isNormalize)
                    {
                        SerializedObject serializedObject = new SerializedObject(audioImporter);
                        SerializedProperty normalizeProperty = serializedObject.FindProperty("m_Normalize");

                        normalizeProperty.boolValue = normalize;

                        serializedObject.ApplyModifiedProperties();
                    }


                    if (platform != "Default")
                    {
                        var sampleSetting = audioImporter.GetOverrideSampleSettings(platform);
                        if (includeProperties.isLoadType)
                        {
                            sampleSetting.loadType = audioImporterSample.loadType;
                        }

                        if (includeProperties.isFormat)
                        {
                            if (platform == "Standalone")
                            {
                                if (audioImporterSample.compressionFormat == AudioCompressionFormat.MP3)
                                {
                                    sampleSetting.compressionFormat = AudioCompressionFormat.Vorbis;
                                }
                                else
                                {
                                    sampleSetting.compressionFormat = audioImporterSample.compressionFormat;
                                }
                            }
                            else
                            {
                                sampleSetting.compressionFormat = audioImporterSample.compressionFormat;
                            }


                        }

                        if (includeProperties.isQuality)
                        {
                            sampleSetting.quality = audioImporterSample.quality;
                        }

                        if (includeProperties.isSampleRateSetting)
                        {
                            sampleSetting.sampleRateSetting = audioImporterSample.sampleRateSetting;
                        }

                        if (includeProperties.isSampleRate)
                        {
                            sampleSetting.sampleRateOverride = audioImporterSample.sampleRateOverride;
                        }
                        audioImporter.SetOverrideSampleSettings(platform, sampleSetting);
                    }
                    else
                    {
                        var sampleSetting = audioImporter.defaultSampleSettings; //audioImporter.GetOverrideSampleSettings(platform);

                        if (includeProperties.isLoadType)
                        {
                            sampleSetting.loadType = audioImporterSample.loadType;
                        }

                        if (includeProperties.isFormat)
                        {
                            //Debug.Log("audioImporterSample.compressionFormat:" + audioImporterSample.compressionFormat);
                            sampleSetting.compressionFormat = audioImporterSample.compressionFormat;
                        }

                        if (includeProperties.isQuality)
                        {
                            sampleSetting.quality = audioImporterSample.quality;
                        }

                        if (includeProperties.isSampleRateSetting)
                        {
                            sampleSetting.sampleRateSetting = audioImporterSample.sampleRateSetting;
                        }

                        if (includeProperties.isSampleRate)
                        {
                            sampleSetting.sampleRateOverride = audioImporterSample.sampleRateOverride;
                        }

                        //audioImporter.SetOverrideSampleSettings(platform, sampleSetting);
                        audioImporter.defaultSampleSettings = sampleSetting;
                    }

                    EditorUtility.SetDirty(audioImporter);
                    audioImporter.SaveAndReimport();

                    yield return new WaitForEndOfFrame();
                }

                yield return new WaitForEndOfFrame();
                var finishTime = EditorApplication.timeSinceStartup;

                Debug.Log("<color=Orange>Effected Audios:" + effectedTextures + " Time:" + (finishTime - startTime) + "</color>");

            }
        }

        private void OnUpdate(bool t)
        {
            if (!t)
            {
                EditorUtility.DisplayProgressBar("Audio Compress", "Compression is in progress...", effectedTextures / total);
            }
            else
            {
                EditorUtility.ClearProgressBar();
            }
        }

        public void ClearSlectedOverride(string[] searchInFolders, int overrideOption)
        {
            List<string> audios = Instance.GetAudioPath(searchInFolders);

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
            foreach (var audioPath in audios)
            {
                AudioImporter audioClip = (AudioImporter)AudioImporter.GetAtPath(audioPath);
                audioClip.ClearSampleSettingOverride(platform);

                EditorUtility.SetDirty(audioClip);
                audioClip.SaveAndReimport();
            }
        }

        public class IncludePropertiesList_Audio
        {
            public bool isForceToMono;
            public bool isNormalize;
            public bool isLoadType;
            public bool isPreloadData;
            public bool isFormat;
            public bool isQuality;
            public bool isSampleRateSetting;
            public bool isSampleRate;

            public IncludePropertiesList_Audio(bool isForceToMono, bool isNormalize, bool isLoadType,
                bool isPreloadData, bool isFormat, bool isQuality, bool isSampleRateSetting, bool isSampleRate)
            {
                this.isForceToMono = isForceToMono;
                this.isNormalize = isNormalize;
                this.isLoadType = isLoadType;
                this.isPreloadData = isPreloadData;
                this.isFormat = isFormat;
                this.isQuality = isQuality;
                this.isSampleRateSetting = isSampleRateSetting;
                this.isSampleRate = isSampleRate;
            }
        }
    }

}
