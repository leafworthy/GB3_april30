using UnityEngine;
using UnityEditor;
using System.IO;

public class CameraCapture
{
    [MenuItem("GameObject/Export Camera View as PNG", false, 0)]
    public static void ExportCameraViewAsPNG()
    {
        Camera targetCamera = Selection.activeGameObject?.GetComponent<Camera>();

        if (targetCamera == null)
        {
	        targetCamera = Camera.main;
            if(targetCamera == null)Debug.LogError("Please select a GameObject with a Camera component in the Hierarchy.");
            return;
        }

        // Use the Game View's current resolution.
        int width = targetCamera.pixelWidth;
        int height = targetCamera.pixelHeight;

        if (width == 0 || height == 0)
        {
            Debug.LogError("The selected camera has a zero-sized output. Please ensure the Game View is visible and set to the correct aspect ratio.");
            return;
        }

        // Create a temporary render texture to capture the camera's output.
        RenderTexture renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);

        // Capture the camera's view into the render texture.
        RenderTexture originalRenderTexture = targetCamera.targetTexture;
        targetCamera.targetTexture = renderTexture;
        targetCamera.Render();

        // Restore the camera's original target texture.
        targetCamera.targetTexture = originalRenderTexture;

        // Convert the render texture to a readable Texture2D.
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture.Apply();

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
