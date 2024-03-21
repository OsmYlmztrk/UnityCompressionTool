using UnityEditor;
using UnityEngine; 

public class EditTexture : MonoBehaviour
{

    [MenuItem("TextureCompressor/Execute")]
    public static void Execute()
    {
        TextureCompressEditor.Instance.ChangeTextureSettings();
    }
}