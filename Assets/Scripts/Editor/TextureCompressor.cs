using Noss.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Presets;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[InitializeOnLoad]
public class TextureCompressor : EditorWindow
{
    [SerializeField] private VisualTreeAsset _tree;
    private Preset preset;


    private ListView _imageListView;
    private List<string> texturePathList = new List<string>();
    private ObjectField objectField;
    private VisualElement choosenImage;
    List<Texture2D> allObjects = new List<Texture2D>();


    [MenuItem("Texture Tool/Compressor")]
    public static void ShowExample()
    {
        TextureCompressor wnd = GetWindow<TextureCompressor>();
        wnd.titleContent = new GUIContent("Compressor");
    }

    public void CreateGUI()
    {
        _tree.CloneTree(rootVisualElement);

        _imageListView = rootVisualElement.Q<ListView>("imageListView");
        objectField = rootVisualElement.Q<ObjectField>("texturePreset");
        choosenImage = rootVisualElement.Q<VisualElement>("choosenImage");

        string[] guids;
        string[] searchInFolders = { "Assets" };
        allObjects = new List<Texture2D>();

        guids = AssetDatabase.FindAssets("t:" + typeof(Texture2D).Name, searchInFolders);

        texturePathList.Clear();
        foreach (string guid in guids)
        {
            texturePathList.Add(AssetDatabase.GUIDToAssetPath(guid));
            allObjects.Add(AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guid)));
        }

        Action<VisualElement, int> bindItem = (item, index) =>
        {
            (item as Label).text = allObjects[index].name;
            item.RegisterCallback<PointerDownEvent, Texture2D>(ChangeChoosenImage, allObjects[index]);
        };

        _imageListView.makeItem = () => new Label();
        _imageListView.bindItem = bindItem;
        _imageListView.itemsSource = allObjects;

        SetupButtonHandler();
    }

    void ChangeChoosenImage(PointerDownEvent evt, Texture2D data)
    {
        choosenImage.style.backgroundImage = new StyleBackground(data);
    }
    //Seçili resmi Presete göre editler
    IEnumerator ChangeTextureSettings()
    {
        var preset = (Preset)objectField.value;

        List<TextureImporter> textures = new List<TextureImporter>();

        foreach (var item in texturePathList)
        {

            TextureImporter t = (TextureImporter)AssetImporter.GetAtPath(item);
            textures.Add(t);

        }
        total = textures.Count;
        int tmp = 0;
        foreach (var texture in textures)
        {
            if (CopyObjectSerialization(preset, texture))
            {
                tmp++;
                Debug.Log(tmp + "/" + total);
            }
            else
            {
                Debug.Log(texture.name + " is not done!");
            }
            EditorUtility.SetDirty(texture);
            texture.SaveAndReimport();
            yield return new WaitForSecondsRealtime(0.1f);
        }

    }

    private void SetupButtonHandler()
    {
        VisualElement root = rootVisualElement;

        var buttons = root.Query<Button>("compressButton");
        buttons.ForEach(RegisterHandler);
    }
    private void RegisterHandler(Button button)
    {
        button.RegisterCallback<ClickEvent>(OnClickButton);
    }
    private void OnClickButton(ClickEvent evt)
    {
        TextureCompressEditor.Instance.ChangeTextureSettings();
    }
    int percent = 0;
    int total = 1;
    private void OnUpdate(bool t)
    {
        if (!t)
        {
            EditorUtility.DisplayProgressBar("Simple Progress Bar", "Doing some work...", percent / total);
        }
        else
        {
            EditorUtility.ClearProgressBar();
        }
    }

    public static bool CopyObjectSerialization(Preset source, UnityEngine.Object target)
    {
        if (!source)
        {
            Debug.LogError("Preset is Null");
            return false;
        }

        return source.ApplyTo(target);
    }

    void CreatePresetAssetFromSelectedTexture()
    {
        TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(Selection.activeObject));

        Preset preset = new Preset(ti);
        AssetDatabase.CreateAsset(preset, "Assets/" + Selection.activeObject.name + ".preset");
    }

}