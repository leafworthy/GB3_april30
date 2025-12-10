using UnityEngine;
using UnityEditor;
using System.IO;

#if UNITY_EDITOR
public class CameraCapture
{
    [MenuItem("GameObject/Export Camera View as PNG", false, 0)]
    public static void ExportCameraViewAsPNG()
    {
        Camera targetCamera = Selection.activeGameObject?.GetComponent<Camera>();

        Debug.Log(SystemInfo.maxTextureSize +  "max");
        // Use the Game View's current resolution.
        int width = 3840*2;
        int height = 2160*2;

        var desc = new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGB32, 24)
        {
	        sRGB = true,
	        useMipMap = false,
	        autoGenerateMips = false,
	        msaaSamples = 1
        };
        RenderTexture renderTexture = new RenderTexture(desc);
        renderTexture.antiAliasing = 1;
        renderTexture.useDynamicScale = false;

        var originalRT = targetCamera.targetTexture;
        targetCamera.allowDynamicResolution = false;
        targetCamera.targetTexture = renderTexture;



        targetCamera.Render();

        RenderTexture.active = renderTexture;

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture.Apply(false, false);

        Debug.Log($"Captured texture size: {texture.width}x{texture.height}");

        // Encode the texture to a PNG and save to a file.
        byte[] bytes = texture.EncodeToPNG();
        string directory = "Assets/ExportedCameraCaptures";
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        string path = $"{directory}/{targetCamera.name}_Capture_{System.DateTime.Now:yyyyMMdd_HHmmss}.png";
        File.WriteAllBytes(path, bytes);
        Debug.Log($"Successfully exported PNG to: {path}");

        // Clean up temporary objects.
        RenderTexture.active = null;
        Object.DestroyImmediate(renderTexture);
        Object.DestroyImmediate(texture);
        AssetDatabase.Refresh();
    }
}
#endif
