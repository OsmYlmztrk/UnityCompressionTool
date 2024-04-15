using Soda.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Soda
{
    [InitializeOnLoad]
    public class AnimationCompressEditor
    {
        private int total = 0;
        private int effectedTextures = 0;

        private static AnimationCompressEditor instance;

        public static AnimationCompressEditor Instance
        {
            get
            {
                if (instance == null)
                    instance = new AnimationCompressEditor();

                return instance;
            }
        }

        public List<string> GetAnimPath(string[] searchInFolders)
        {
            string[] guids;
            List<string> meshesPathList = new List<string>();

            guids = AssetDatabase.FindAssets("t:" + typeof(Animation).Name, searchInFolders);

            foreach (string guid in guids)
            {
                meshesPathList.Add(AssetDatabase.GUIDToAssetPath(guid));
            }

            return meshesPathList;
        }
        public void ChangeAnimationSettings(ModelImporterAnimationCompression option, string[] searchInFolders)
        {
            var meshes = GetAnimPath(searchInFolders);
            total = meshes.Count;
            effectedTextures = 0;
            MyEditorCoroutines.Execute(Delay(), OnUpdate);

            IEnumerator Delay()
            {
                var startTime = EditorApplication.timeSinceStartup;

                foreach (var item in meshes)
                {
                    effectedTextures++;

                    ModelImporter modelImporter = (ModelImporter)ModelImporter.GetAtPath(item);
                    modelImporter.animationCompression = option;

                    EditorUtility.SetDirty(modelImporter);
                    modelImporter.SaveAndReimport();

                    yield return new WaitForEndOfFrame();
                }

                yield return new WaitForEndOfFrame();
                var finishTime = EditorApplication.timeSinceStartup;

                Debug.Log("<color=Orange>Effected Animations:" + effectedTextures + " Time:" + (finishTime - startTime) + "</color>");
            }
        }

        private void OnUpdate(bool t)
        {
            if (!t)
            {
                EditorUtility.DisplayProgressBar("Animation Compress", "Compression is in progress...", effectedTextures / total);
            }
            else
            {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}
