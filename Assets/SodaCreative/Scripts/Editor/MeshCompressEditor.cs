using Soda.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Soda
{
    [InitializeOnLoad]
    public class MeshCompressEditor
    {
        private int total = 0;
        private int effectedTextures = 0;

        private static MeshCompressEditor instance;

        public static MeshCompressEditor Instance
        {
            get
            {
                if (instance == null)
                    instance = new MeshCompressEditor();

                return instance;
            }
        }

        public List<string> GetMeshesPath(string[] searchInFolders)
        {
            string[] guids;
            List<string> meshesPathList = new List<string>();

            guids = AssetDatabase.FindAssets("t:" + typeof(Mesh).Name, searchInFolders);

            foreach (string guid in guids)
            {
                meshesPathList.Add(AssetDatabase.GUIDToAssetPath(guid));
            }

            return meshesPathList;
        }

        public void ChangeMeshSettings(ModelImporterMeshCompression option, string[] searchInFolders)
        {
            var meshes = GetMeshesPath(searchInFolders);
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
                    modelImporter.meshCompression = option;

                    EditorUtility.SetDirty(modelImporter);
                    modelImporter.SaveAndReimport();

                    yield return new WaitForEndOfFrame();
                }

                yield return new WaitForEndOfFrame();
                var finishTime = EditorApplication.timeSinceStartup;

                Debug.Log("<color=Orange>Effected Meshes:" + effectedTextures + " Time:" + (finishTime - startTime) + "</color>");
            }
        }

        private void OnUpdate(bool t)
        {
            if (!t)
            {
                EditorUtility.DisplayProgressBar("Mesh Compress", "Compression is in progress...", effectedTextures / total);
            }
            else
            {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}
