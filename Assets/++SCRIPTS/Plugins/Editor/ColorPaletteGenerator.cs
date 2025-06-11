using UnityEngine;
using UnityEditor;
using __SCRIPTS;
using System.IO;

public static class ColorPaletteGenerator
{
    [MenuItem("Tools/Generate Color Palettes")]
    public static void GenerateColorPalettes()
    {
        const int paletteCount = 10;
        const string outputPath = "Assets/Resources/ColorPalettes/";

        if (!AssetDatabase.IsValidFolder(outputPath.TrimEnd('/')))
            Directory.CreateDirectory(outputPath);

        for (int i = 0; i < paletteCount; i++)
        {
            var palette = ScriptableObject.CreateInstance<HouseColorScheme>();

            // Generate a base hue and some harmonious variants
            float baseHue = Random.value;
            float hueOffset = Random.Range(0.05f, 0.15f); // analogous spacing

            Color baseColor = Color.HSVToRGB(baseHue, Random.Range(0.4f, 0.8f), Random.Range(0.7f, 1f));
            Color complementary = Color.HSVToRGB((baseHue + 0.5f) % 1f, Random.Range(0.4f, 0.8f), Random.Range(0.6f, 1f));
            Color analogous1 = Color.HSVToRGB((baseHue + hueOffset) % 1f, Random.Range(0.5f, 0.9f), Random.Range(0.7f, 1f));
            Color analogous2 = Color.HSVToRGB((baseHue - hueOffset + 1f) % 1f, Random.Range(0.5f, 0.9f), Random.Range(0.7f, 1f));

            // Main color assignments
            palette.walls_exterior = baseColor;
            palette.walls_interior = analogous1;
            palette.walls_side_room = analogous2;

            palette.rug_main_room = complementary;
            palette.rug_side_room = Color.Lerp(analogous1, complementary, 0.5f);

            palette.kitchen_floor = ShiftBrightness(baseColor, -0.2f);
            palette.kitchen_counters = ShiftSaturation(analogous1, -0.2f);
            palette.kitchen_table = ShiftBrightness(analogous1, 0.2f);
            palette.kitchen_fence = complementary;

            palette.bedroom_floor = ShiftSaturation(baseColor, -0.3f);
            palette.back_door = complementary;
            palette.kitchen_door = analogous2;
            palette.front_door = analogous1;
            palette.bedroom_door = complementary;

            // Previously hardcoded — now randomized with harmony
            palette.bathroom_door = ShiftBrightness(analogous2, -0.15f);
            palette.tv = ShiftSaturation(baseColor, -0.8f); // very low saturation = nearly grayscale

            palette.lampshade = ShiftBrightness(baseColor, 0.3f);
            palette.couch = Color.Lerp(baseColor, complementary, 0.3f);

            string fileName = $"ColorPalette_{i + 1}.asset";
            AssetDatabase.CreateAsset(palette, Path.Combine(outputPath, fileName));
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"{paletteCount} color palettes generated in {outputPath}");
    }

    private static Color ShiftBrightness(Color color, float delta)
    {
        Color.RGBToHSV(color, out float h, out float s, out float v);
        v = Mathf.Clamp01(v + delta);
        return Color.HSVToRGB(h, s, v);
    }

    private static Color ShiftSaturation(Color color, float delta)
    {
        Color.RGBToHSV(color, out float h, out float s, out float v);
        s = Mathf.Clamp01(s + delta);
        return Color.HSVToRGB(h, s, v);
    }
}
