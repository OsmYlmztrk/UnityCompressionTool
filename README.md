
# Unity Texture Compression Tool

This tool is designed to compress and optimise texture files used in Unity projects. This compression process reduces project size and improves game performance while maintaining visual quality.


## Youtube Overiew
[![Soda Gaming - Asset Compress Tool](https://img.youtube.com/vi/WcqipOI2PIw/0.jpg)](https://www.youtube.com/watch?v=WcqipOI2PIw)

## Screenshots

![Compress all assets in your project at once](https://github.com/OsmYlmztrk/TextureCompress/assets/119442331/1d109e38-88b9-475e-840a-8ab14d5b8116)

![AreYouSure?](https://github.com/OsmYlmztrk/TextureCompress/assets/119442331/d6aad9df-9853-4b77-8be7-ed813152ec1a)

  
## Features

- Texture, Mesh, Audio, Animation compress operations
- Folder based selections
- User Friendly interface
- Default, Standalone, Android, iOS platform support

  
## Road Map

- ~~Compression Levels: You can choose between different compression levels, allowing you to balance performance and quality.~~

- ~~Platform Support: Customised compression options are available for different platforms, so you can optimise for each platform.~~

- ~~Editor code will be added for ease of use.~~
  
- Auto Update: Compression process can be started automatically when texture files are changed or new texture is added.

  
## Methods

```c##
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

  
