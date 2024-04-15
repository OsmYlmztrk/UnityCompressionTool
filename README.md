
# Unity Texture Compression Tool

This tool is designed to compress and optimise texture files used in Unity projects. This compression process reduces project size and improves game performance while maintaining visual quality.


## Ekran Görüntüleri

![SS](https://prnt.sc/6mafUXNQY9N4) 

  
## Road Map

- Compression Levels: You can choose between different compression levels, allowing you to balance performance and quality.

- Platform Support: Customised compression options are available for different platforms, so you can optimise for each platform.

- Auto Update: Compression process can be started automatically when texture files are changed or new texture is added.

- Editor code will be added for ease of use.

  
## Methods

```c#
//Just call this
TextureCompressEditor.Instance.ChangeTextureSettings();

//Textures under this folder will be affected.
string[] searchInFolders = { "Assets/Textures" };// For now
List<string> textures = Instance.GetTexturesPath(searchInFolders);

//You can find the changed properties in the TextureSettings class.
public int maxTextureSize;
public TextureImporterFormat textureFormat;
public TextureImporterCompression textureCompression;
[Range(0, 100)] 
public int compressionQuality;
public bool crunchedCompression;
public bool mipmapEnabled;
```

  
## Lisans

[MIT](https://choosealicense.com/licenses/mit/)

  
