using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Soda.Utilities;
using System;
using System.Linq;

namespace Soda
{
    [InitializeOnLoad]
    public class CompressEditor : EditorWindow
    {

        private static CompressEditor _instance;
        public static CompressEditor GetInstance()
        {
            if (_instance == null)
            {
                _instance = new CompressEditor();
            }
            return _instance;
        }

        #region Texture Properties
        int selGridInt_override = 0;
        string[] selOverrideStrings = { " Default", " Standalone", " Android", " iOS" };

        int toolbarInt = 0;
        string texturePath = "Assets";
        string labelPath_texture = "Assets/";

        string[] toolbarStrings = new string[] { "Texture", "Mesh", "Audio", "Animations" };

        int _selectedMaxSizeOption = 4;
        string[] _maxSizeOptions = new string[10] { "32", "64", "128", "256", "512", "1024", "2048", "4096", "8192", "16384" };

        int _selectedResizeAlgorithm = 0;
        string[] _resizeAlgorithms = new string[2] { "Bilinear", "Mitchell" };

        int _selectedTextureImporterFormat = 75;
        List<string> _textureImporterFormat = new List<string>();
        string[] _textureImporterFormatArray = new string[1] { "Automatic" };

        int _selectedTextureImporterCompression = 0;
        string[] _TextureImporterCompression = new string[4] { "None", "Low Quality", "Normal Quality", "High Quality" };

        int compressionQualityValue = 50;

        bool toggleMaxTextureSize = false;
        bool resizeAlgorithm = false;
        bool textureImporterFormat = false;
        bool textureImporterCompression = false;
        bool crunchedCompression = true;

        int maxTextureSizeValue = 512;
        TextureImporterFormat choosenFormat = TextureImporterFormat.Automatic;
        #endregion

        #region Mesh Properties

        string MeshPath = "Assets";
        string labelPath_mesh = "Assets/";

        bool toggleMeshCompression = false;
        int _selectedMeshCompression = 0;
        string[] _MeshCompressionOptions = new string[4] { "Off", "Low", "Medium", "High" };

        #endregion

        #region Audio Properties
        int selGridInt_override_Audio = 0;

        string AudioPath = "Assets";
        string labelPath_Audio = "Assets/";

        bool toggleForceToMono = false;
        bool toggleForceToMonoSelected = false;

        bool toggleLoadType = false;
        int _selectedLoadType = 0;
        string[] _loadTypeOptions = new string[3] { "Decompress On Load", "Compressed In Memory", "Streaming" };

        bool togglePreloadAudioData = false;
        bool preloadAudioDataValue = false;

        bool toggleCompressionFormat = false;
        int _selectedCompressionFormat = 1;
        string[] _compressionFormatOptions = new string[4] { "PCM", "Vorbis", "ADPCM", "MP3" };

        bool toggleQuality = false;
        int audioQuality = 100;

        bool toggleSampleRateSetting = false;
        int _selectedSampleRateSetting = 0;
        string[] _SampleRateSettingOptions = new string[3] { "Preserve Sample Rate", "Optimize Sample Rate", "Override Sample Rate" };

        bool toggleSampleRate = false;
        int _selectedSampleRate = 3;
        string[] _SampleRateOptions = new string[7] { "8,000 Hz", "11,025 Hz", "22,050 Hz", "44,100 Hz", "48,000 Hz", "96,000 Hz", "192,000 Hz" };

        #endregion

        #region Animation Properties
        string AnimPath = "Assets";
        string labelPath_Anim = "Assets/";
        bool toggleAnimationCompression = false;
        int _selectedAnimationCompression = 0;
        string[] _AnimationCompressionOptions = new string[2] { "Off", "Keyframe Reduction"/*, "Keyframe Reduction and Compression", "Optimal" */};

        #endregion

        [MenuItem("Window/Soda Creative/Compress Tool")]
        static void Init()
        {
            EditorWindow.GetWindow(typeof(CompressEditor), false, "Compress Tool", true).Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            GUIHelperSoda.ShowHeaderLogo();
            EditorGUILayout.Space();


            toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings);

            if (toolbarInt == 0)
            {
                TextureTab();
            }
            else if (toolbarInt == 1)
            {
                MeshTab();
            }
            else if (toolbarInt == 2)
            {
                AudioTab();
            }
            else if (toolbarInt == 3)
            {
                AnimationTab();
            }

            //
            GUILayout.Label("*The compression process may vary depending on the project size.\n  Therefore, we recommend that you do it in small pieces and definitely take a BACKUP.", EditorStyles.miniBoldLabel);
        }
        #region Texture Tab
        private void TextureTab()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Texture Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            #region Folder Path Button
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Folder Path", GUILayout.Width(80)))
            { 

                texturePath = EditorUtility.OpenFolderPanel("Choose Folder", "Assets", "");

                if (string.IsNullOrEmpty(texturePath))
                {
                    texturePath = "Assets";
                }

                string[] result = texturePath.Split('/');

                string newPath = "";
                bool t = false;
                foreach (string s in result)
                {
                    if (s == "Assets")
                    {
                        t = true;
                    }

                    if (t)
                    {
                        newPath += s + "/";
                    }
                }
                labelPath_texture = newPath;
            }
            GUILayout.Label(labelPath_texture);
            GUILayout.EndHorizontal();
            #endregion

            EditorGUILayout.Space();

            #region Override
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Override", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();

            selGridInt_override = GUILayout.SelectionGrid(selGridInt_override, selOverrideStrings, 4, "radio");

            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Remove Selected Override"))
            { 

                var areYouSure = EditorUtility.DisplayDialog("Are you sure!", "This will also include all subfolders. You have to be careful. \n\nThis process may take some time.", "OK", "Cancel");
                if (areYouSure)
                {
                    string[] searchInFolders = { labelPath_texture };
                    TextureCompressEditor.Instance.ClearSlectedOverride(searchInFolders, selGridInt_override);
                }
            }
            EditorGUILayout.EndVertical();

            #endregion

            EditorGUILayout.Space();

            #region Max Texture Size
            bool showHelpBoxMaxTextureSize = toggleMaxTextureSize;
            if (showHelpBoxMaxTextureSize)
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            toggleMaxTextureSize = GUILayout.Toggle(toggleMaxTextureSize, "Max Texture Size");
            if (toggleMaxTextureSize)
            {
                EditorGUI.BeginChangeCheck();
                _selectedMaxSizeOption = EditorGUILayout.Popup("Max Size", _selectedMaxSizeOption, _maxSizeOptions);

                if (EditorGUI.EndChangeCheck())
                {
                    maxTextureSizeValue = Convert.ToInt32(_maxSizeOptions[_selectedMaxSizeOption]);
                    //Debug.Log(_maxSizeOptions[_selectedMaxSizeOption]);
                }
                EditorGUILayout.Space();

            }

            if (showHelpBoxMaxTextureSize)
                GUILayout.EndVertical();
            #endregion

            #region Resize Algorithm
            bool showHelpBoxresizeAlgorithm = resizeAlgorithm;
            if (showHelpBoxresizeAlgorithm)
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            resizeAlgorithm = GUILayout.Toggle(resizeAlgorithm, "Resize Algorithm");
            if (resizeAlgorithm)
            {
                EditorGUI.BeginChangeCheck();
                _selectedResizeAlgorithm = EditorGUILayout.Popup("Resize Algorithm", _selectedResizeAlgorithm, _resizeAlgorithms);

                if (EditorGUI.EndChangeCheck())
                {
                    
                }
                EditorGUILayout.Space();
            }

            if (showHelpBoxresizeAlgorithm)
                GUILayout.EndVertical();
            #endregion

            #region Format
            bool showHelpBoxtextureImporterFormat = textureImporterFormat;
            if (showHelpBoxtextureImporterFormat)
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            textureImporterFormat = GUILayout.Toggle(textureImporterFormat, "Format");
            if (textureImporterFormat)
            {
                EditorGUI.BeginChangeCheck();

                if (_textureImporterFormat.Count == 0 || _textureImporterFormatArray.Length == 0)
                {
                    var enums = Enum.GetValues(typeof(TextureImporterFormat)).Cast<TextureImporterFormat>().ToList();
                    foreach (var item in enums)
                    {
                        _textureImporterFormat.Add(item.ToString());
                    }

                    _textureImporterFormatArray = _textureImporterFormat.ToArray();
                }

                _selectedTextureImporterFormat = EditorGUILayout.Popup("Format", _selectedTextureImporterFormat, _textureImporterFormatArray);
                GUILayout.Label("*If the format you selected is not in the platform settings, the format cannot be applied.", EditorStyles.miniBoldLabel);
                if (EditorGUI.EndChangeCheck())
                {
                    if (Enum.TryParse(_textureImporterFormatArray[_selectedTextureImporterFormat], out TextureImporterFormat myStatus))
                    {
                        choosenFormat = myStatus;
                    }


                }
            }

            if (showHelpBoxtextureImporterFormat)
                GUILayout.EndVertical();
            #endregion

            #region Texture Importer Compression
            bool showHelpBoxCompression = textureImporterCompression;
            if (showHelpBoxCompression)
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            textureImporterCompression = GUILayout.Toggle(textureImporterCompression, "Compression");
            if (textureImporterCompression)
            {
                EditorGUI.BeginChangeCheck();
                _selectedTextureImporterCompression = EditorGUILayout.Popup("Compression", _selectedTextureImporterCompression, _TextureImporterCompression);

                crunchedCompression = GUILayout.Toggle(crunchedCompression, "Use Crunch Compression");

                if (crunchedCompression)
                {
                    compressionQualityValue = EditorGUILayout.IntSlider("Compressor Quality", compressionQualityValue, 0, 100);
                }

                if (EditorGUI.EndChangeCheck())
                {

                }
            }
            if (showHelpBoxCompression)
                GUILayout.EndVertical();
            #endregion

            EditorGUILayout.Space();

            if (GUILayout.Button("Compress Textures"))
            {
                var areYouSure = EditorUtility.DisplayDialog("Compress Textures", "This will also include all subfolders. You have to be careful. \n\nThis process may take some time. Are you sure!", "OK", "Cancel");
                if (areYouSure)
                {
                    //Debug.Log("_resizeAlgorithms:" + _resizeAlgorithms.Length+"-"+_selectedResizeAlgorithm);
                    //Debug.Log("_textureImporterFormatArray:" + _textureImporterFormatArray.Length);
                    //Debug.Log("_TextureImporterCompression:" + _TextureImporterCompression.Length);

                    ApplyChanges(
                        labelPath_texture,//Path
                        selGridInt_override,//Override platform
                        maxTextureSizeValue, //Max size
                        _resizeAlgorithms[_selectedResizeAlgorithm],//Resize Algorithm
                        _textureImporterFormatArray[_selectedTextureImporterFormat % _textureImporterFormatArray.Length],//Format
                        _TextureImporterCompression[_selectedTextureImporterCompression],//Compression
                        crunchedCompression,//useCrunchCompression
                        compressionQualityValue //Compressor Quality
                        );
                }
            }

            GUILayout.EndVertical();
        }

        private void ApplyChanges(
            string path,
            int overrideOption,
            int maxTextureSize,
            string resizeAlgorithm,
            string format,
            string Compression,
            bool useCrunchCompression,
            int compressorQuality
            )
        {

            TextureResizeAlgorithm resAlgorithm = TextureResizeAlgorithm.Bilinear;
            switch (resizeAlgorithm)
            {
                case "Bilinear":
                    resAlgorithm = TextureResizeAlgorithm.Bilinear;
                    break;
                case "Mitchell":
                    resAlgorithm = TextureResizeAlgorithm.Mitchell;
                    break;

            }

            TextureImporterCompression compression = TextureImporterCompression.Uncompressed;
            switch (Compression)
            {
                case "None":
                    compression = TextureImporterCompression.Uncompressed;
                    break;
                case "Low Quality":
                    compression = TextureImporterCompression.CompressedLQ;
                    break;
                case "Normal Quality":
                    compression = TextureImporterCompression.Compressed;
                    break;
                case "High Quality":
                    compression = TextureImporterCompression.CompressedHQ;
                    break;
            }

            string platform = "Default";
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

            TextureCompressEditor.TextureSettings sett = new TextureCompressEditor.TextureSettings(
                 maxTextureSize,
                 resAlgorithm,
                 choosenFormat,
                 compression,
                 compressorQuality,
                 useCrunchCompression);
            BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
            string[] searchInFolders = { path };

            bool compressQuality = (textureImporterCompression && crunchedCompression);
            TextureCompressEditor.IncludePropertiesList_Texture propertiesList = new TextureCompressEditor.IncludePropertiesList_Texture(
                toggleMaxTextureSize, this.resizeAlgorithm, textureImporterFormat, textureImporterCompression, compressQuality, crunchedCompression);


            TextureCompressEditor.Instance.ChangeTextureSettings(sett, searchInFolders, buildTarget, platform, propertiesList);
        }

        #endregion

        #region Mesh Tab

        private void MeshTab()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Mesh Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            #region Folder Path Button
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Folder Path", GUILayout.Width(80)))
            {
                
                MeshPath = EditorUtility.OpenFolderPanel("Choose Folder", "Assets", "");
                if (string.IsNullOrEmpty(MeshPath))
                {
                    MeshPath = "Assets";
                }
                string[] result = MeshPath.Split('/');



                string newPath = "";
                bool t = false;
                foreach (string s in result)
                {
                    if (s == "Assets")
                    {
                        t = true;
                    }

                    if (t)
                    {
                        newPath += s + "/";
                    }
                }
                labelPath_mesh = newPath;
            }
            GUILayout.Label(labelPath_mesh);
            GUILayout.EndHorizontal();
            #endregion

            #region 
            EditorGUILayout.Space();

            bool showHelpBoxMeshCompression = toggleMeshCompression;
            if (showHelpBoxMeshCompression)
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            toggleMeshCompression = GUILayout.Toggle(toggleMeshCompression, "Mesh Compression");
            if (toggleMeshCompression)
            {
                _selectedMeshCompression = EditorGUILayout.Popup("Mesh Compression", _selectedMeshCompression, _MeshCompressionOptions);

                EditorGUILayout.Space();
            }
            if (showHelpBoxMeshCompression)
                GUILayout.EndVertical();



            #endregion

            EditorGUILayout.Space();
            if (toggleMeshCompression)
            {
                if (GUILayout.Button("Compress Meshes"))
                {
                   
                    var areYouSure = EditorUtility.DisplayDialog("Compress Meshes", "This will also include all subfolders. You have to be careful. \n\nThis process may take some time. Are you sure!", "OK", "Cancel");
                    string[] searchInFolders = { labelPath_mesh };

                    ModelImporterMeshCompression option = ModelImporterMeshCompression.Off;

                    switch (_selectedMeshCompression)
                    {
                        case 0:
                            option = ModelImporterMeshCompression.Off;
                            break;
                        case 1:
                            option = ModelImporterMeshCompression.Low;
                            break;
                        case 2:
                            option = ModelImporterMeshCompression.Medium;
                            break;
                        case 3:
                            option = ModelImporterMeshCompression.High;
                            break;
                    }

                    if (areYouSure)
                    {
                        MeshCompressEditor.Instance.ChangeMeshSettings(option, searchInFolders);
                    }

                }
            }
            EditorGUILayout.EndVertical();
        }

        #endregion

        #region Animation Tab

        private void AnimationTab()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Animation Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            #region Folder Path Button
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Folder Path", GUILayout.Width(80)))
            {
               
                AnimPath = EditorUtility.OpenFolderPanel("Choose Folder", "Assets", "");

                if (string.IsNullOrEmpty(AnimPath))
                {
                    AnimPath = "Assets";
                }

                string[] result = AnimPath.Split('/');

                string newPath = "";
                bool t = false;
                foreach (string s in result)
                {
                    if (s == "Assets")
                    {
                        t = true;
                    }

                    if (t)
                    {
                        newPath += s + "/";
                    }
                }
                labelPath_Anim = newPath;
            }
            GUILayout.Label(labelPath_Anim);
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
            #endregion

            bool showHelpBoxAnimCompression = toggleAnimationCompression;
            if (showHelpBoxAnimCompression)
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            toggleAnimationCompression = GUILayout.Toggle(toggleAnimationCompression, "Animation Compression");
            if (toggleAnimationCompression)
            {
                _selectedAnimationCompression = EditorGUILayout.Popup("Animation Compression", _selectedAnimationCompression, _AnimationCompressionOptions);

                EditorGUILayout.Space();
            }
            if (showHelpBoxAnimCompression)
                GUILayout.EndVertical();

            EditorGUILayout.Space();
            if (toggleAnimationCompression)
            {
                if (GUILayout.Button("Compress Animations"))
                {

                    var areYouSure = EditorUtility.DisplayDialog("Compress Animations", "This will also include all subfolders. You have to be careful. \n\nThis process may take some time. Are you sure!", "OK", "Cancel");
                    string[] searchInFolders = { labelPath_Anim };

                    ModelImporterAnimationCompression option = ModelImporterAnimationCompression.Off;

                    switch (_selectedAnimationCompression)
                    {
                        case 0:
                            option = ModelImporterAnimationCompression.Off;
                            break;
                        case 1:
                            option = ModelImporterAnimationCompression.KeyframeReduction;
                            break;
                        case 2:
                            option = ModelImporterAnimationCompression.KeyframeReductionAndCompression;
                            break;
                        case 3:
                            option = ModelImporterAnimationCompression.Optimal;
                            break;
                    }

                    if (areYouSure)
                    {
                        AnimationCompressEditor.Instance.ChangeAnimationSettings(option, searchInFolders);
                    }
                }
            }

            EditorGUILayout.EndVertical();
        }
        #endregion

        #region Audio Tab

        string platform = "Default";
        AudioClipLoadType loadType = AudioClipLoadType.DecompressOnLoad;
        bool normalize = false;
        AudioCompressionFormat compressionFormat = AudioCompressionFormat.Vorbis;
        float quality_audio = 1f;
        AudioSampleRateSetting audioSampleRateSetting = AudioSampleRateSetting.PreserveSampleRate;
        uint audioImporterSampleSetting = 44100;

        private void AudioTab()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Audio Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Folder Path", GUILayout.Width(80)))
            {
                AudioPath = EditorUtility.OpenFolderPanel("Choose Folder", "Assets", "");

                if (string.IsNullOrEmpty(AudioPath))
                {
                    AudioPath = "Assets";
                }

                string[] result = AudioPath.Split('/');

                string newPath = "";
                bool t = false;
                foreach (string s in result)
                {
                    if (s == "Assets")
                    {
                        t = true;
                    }

                    if (t)
                    {
                        newPath += s + "/";
                    }
                }
                labelPath_Audio = newPath;
            }
            GUILayout.Label(labelPath_Audio);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            #region Override
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Override", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();

            selGridInt_override_Audio = GUILayout.SelectionGrid(selGridInt_override_Audio, selOverrideStrings, 4, "radio");

            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Remove Selected Override"))
            { 

                var areYouSure = EditorUtility.DisplayDialog("Are you sure!", "This will also include all subfolders. You have to be careful. \n\nThis process may take some time.", "OK", "Cancel");
                if (areYouSure)
                {
                    string[] searchInFolders = { labelPath_Audio };
                    AudioCompressEditor.Instance.ClearSlectedOverride(searchInFolders, selGridInt_override_Audio);
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            #endregion



            switch (selGridInt_override_Audio)
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

            #region
            bool showHelpBoxForceToMono = toggleForceToMono;
            if (showHelpBoxForceToMono)
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            toggleForceToMono = GUILayout.Toggle(toggleForceToMono, "Force To Mono");
            if (toggleForceToMono)
            {
                toggleForceToMonoSelected = EditorGUILayout.Toggle("Force To Mono", toggleForceToMonoSelected);

                if (toggleForceToMonoSelected)
                    normalize = EditorGUILayout.Toggle("Normalize", normalize);
            }

            if (showHelpBoxForceToMono)
                EditorGUILayout.EndVertical();
            #endregion

            #region
            bool showHelpBoxLoadType = toggleLoadType;
            if (showHelpBoxLoadType)
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            toggleLoadType = GUILayout.Toggle(toggleLoadType, "Load Type");
            if (toggleLoadType)
            {
                EditorGUI.BeginChangeCheck();
                _selectedLoadType = EditorGUILayout.Popup("Load Type", _selectedLoadType, _loadTypeOptions);

                if (EditorGUI.EndChangeCheck())
                {
                    switch (_selectedLoadType)
                    {
                        case 0:
                            loadType = AudioClipLoadType.DecompressOnLoad;
                            break;
                        case 1:
                            loadType = AudioClipLoadType.CompressedInMemory;
                            break;
                        case 2:
                            loadType = AudioClipLoadType.Streaming;
                            break;
                    }
                }

                togglePreloadAudioData = GUILayout.Toggle(togglePreloadAudioData, "Preload Data");
                if (togglePreloadAudioData)
                {
                    if (_selectedLoadType != 2)
                    {
                        preloadAudioDataValue = EditorGUILayout.Toggle("Preload Audio Data", preloadAudioDataValue);
                    }
                    else
                    {
                        preloadAudioDataValue = false;
                    }


                }
            }

            if (showHelpBoxLoadType)
                EditorGUILayout.EndVertical();
            #endregion

            #region
            bool showHelpBoxCompressionFormat = toggleCompressionFormat;
            if (showHelpBoxCompressionFormat)
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            toggleCompressionFormat = GUILayout.Toggle(toggleCompressionFormat, "Compression Format");

            if (toggleCompressionFormat)
            {
                EditorGUI.BeginChangeCheck();
                _selectedCompressionFormat = EditorGUILayout.Popup("Compression Format", _selectedCompressionFormat, _compressionFormatOptions);
                //Debug.Log("_selectedCompressionFormat:" + _selectedCompressionFormat);

                if (EditorGUI.EndChangeCheck())
                {
                    switch (_selectedCompressionFormat)
                    {
                        case 0:
                            compressionFormat = AudioCompressionFormat.PCM;
                            break;
                        case 1:
                            compressionFormat = AudioCompressionFormat.Vorbis;
                            break;
                        case 2:
                            compressionFormat = AudioCompressionFormat.ADPCM;
                            break;
                        case 3:
                            compressionFormat = AudioCompressionFormat.MP3;
                            break;
                    }
                }
            }

            if (showHelpBoxCompressionFormat)
                EditorGUILayout.EndVertical();
            #endregion

            #region
            bool showHelpBoxQuality = toggleQuality;
            if (showHelpBoxQuality)
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            toggleQuality = GUILayout.Toggle(toggleQuality, "Quality");

            if (toggleQuality)
            {
                audioQuality = EditorGUILayout.IntSlider("Quality", audioQuality, 0, 100);
                quality_audio = audioQuality;
            }

            if (showHelpBoxQuality)
                EditorGUILayout.EndVertical();
            #endregion

            #region
            bool showHelpBoxSampleRateSetting = toggleSampleRateSetting;
            if (showHelpBoxSampleRateSetting)
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            toggleSampleRateSetting = GUILayout.Toggle(toggleSampleRateSetting, "Sample Rate Setting");

            if (toggleSampleRateSetting)
            {
                EditorGUI.BeginChangeCheck();
                _selectedSampleRateSetting = EditorGUILayout.Popup("Sample Rate Setting", _selectedSampleRateSetting, _SampleRateSettingOptions);

                if (EditorGUI.EndChangeCheck())
                {
                    switch (_selectedSampleRateSetting)
                    {
                        case 0:
                            audioSampleRateSetting = AudioSampleRateSetting.PreserveSampleRate;
                            break;
                        case 1:
                            audioSampleRateSetting = AudioSampleRateSetting.OptimizeSampleRate;
                            break;
                        case 2:
                            audioSampleRateSetting = AudioSampleRateSetting.OverrideSampleRate;
                            break;
                    }
                }

                if (_selectedSampleRateSetting == 2)
                {
                    _selectedSampleRate = EditorGUILayout.Popup("Sample Rate", _selectedSampleRate, _SampleRateOptions);
                    toggleSampleRate = true;
                    switch (_selectedSampleRate)
                    {
                        case 0:
                            audioImporterSampleSetting = 8000;
                            break;
                        case 1:
                            audioImporterSampleSetting = 11025;
                            break;
                        case 2:
                            audioImporterSampleSetting = 22050;
                            break;
                        case 3:
                            audioImporterSampleSetting = 44100;
                            break;
                        case 4:
                            audioImporterSampleSetting = 48000;
                            break;
                        case 5:
                            audioImporterSampleSetting = 96000;
                            break;
                        case 6:
                            audioImporterSampleSetting = 192000;
                            break;
                    }
                }
                else
                {
                    audioImporterSampleSetting = 44100;
                }
            }

            if (showHelpBoxSampleRateSetting)
                EditorGUILayout.EndVertical();
            #endregion


            if (GUILayout.Button("Compress Audio"))
            { 

                AudioCompressEditor.IncludePropertiesList_Audio includeProperties = new AudioCompressEditor.IncludePropertiesList_Audio
                    (toggleForceToMono, toggleForceToMono, toggleLoadType,
                    togglePreloadAudioData, toggleCompressionFormat, toggleQuality,
                    toggleSampleRateSetting, toggleSampleRate);


                var areYouSure = EditorUtility.DisplayDialog("Compress Audio", "This will also include all subfolders. You have to be careful. \n\nThis process may take some time. Are you sure!", "OK", "Cancel");
                string[] searchInFolders = { labelPath_Audio };
                AudioImporterSampleSettings audioImporterSample = new AudioImporterSampleSettings();

                audioImporterSample.loadType = loadType;
                audioImporterSample.compressionFormat = compressionFormat;
                audioImporterSample.quality = quality_audio / 100f;
                audioImporterSample.sampleRateSetting = audioSampleRateSetting;
                audioImporterSample.sampleRateOverride = audioImporterSampleSetting;

                if (areYouSure)
                {
                    AudioCompressEditor.Instance.ChangeAudioSettings(toggleForceToMonoSelected, normalize, preloadAudioDataValue,
                        audioImporterSample, searchInFolders, platform, includeProperties);
                }

            }

            EditorGUILayout.EndVertical();
        }
        #endregion

    }
}