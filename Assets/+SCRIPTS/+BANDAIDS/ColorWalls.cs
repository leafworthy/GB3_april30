using Sirenix.OdinInspector;
using UnityEngine;

namespace __SCRIPTS
{
    public class ColorWalls : MonoBehaviour
    {
        [BoxGroup("Color Settings")]
        public Color WallTint = Color.white;
        [BoxGroup("Color Settings")]
        public Color TrimTint = Color.white;
        [BoxGroup("Color Settings")]
        public Color GlassTint = Color.white;
        [BoxGroup("Color Settings")]
        public Color DoorTint = Color.white;
        [BoxGroup("Color Settings")]
        public Color RoofTint = Color.white;
        [BoxGroup("Color Settings")]
        public Color AccentTint = Color.white;

        [BoxGroup("Randomization Settings")]
        [Range(0f, 1f)]
        public float MinSaturation = 0.3f;

        [BoxGroup("Randomization Settings")]
        [Range(0f, 1f)]
        public float MaxSaturation = 0.8f;

        [BoxGroup("Randomization Settings")]
        [Range(0f, 1f)]
        public float MinBrightness = 0.4f;

        [BoxGroup("Randomization Settings")]
        [Range(0f, 1f)]
        public float MaxBrightness = 0.9f;

        [BoxGroup("Randomization Settings")]
        public bool UseWarmColors = false;

        [BoxGroup("Randomization Settings")]
        public bool UseCoolColors = false;

        private readonly string NoColor_Tag = "dontcolor";
        private readonly string Wall_Tag = "wall";
        private readonly string Trim_Tag = "trim";
        private readonly string Glass_Tag = "glass";
        private readonly string Door_Tag = "door";
        private readonly string Accent_Tag = "accent";
        private readonly string Roof_Tag = "roof";

        private void OnEnable()
        {
            Refresh();
        }

        private void Update()
        {
            if (!Application.isPlaying) Refresh();
        }

        [Button("Refresh Colors")]
        public void Refresh()
        {
            var spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var spriteRenderer in spriteRenderers)
            {
                if (spriteRenderer.CompareTag(NoColor_Tag))
                {
                    spriteRenderer.color = Color.white;
                    continue;
                }

                if (spriteRenderer.CompareTag(Wall_Tag))
                {
                    spriteRenderer.color = WallTint;
                    continue;
                }

                if (spriteRenderer.CompareTag(Trim_Tag))
                {
                    spriteRenderer.color = TrimTint;
                    continue;
                }

                if (spriteRenderer.CompareTag(Glass_Tag))
                {
                    spriteRenderer.color = GlassTint;
                    continue;
                }

                if (spriteRenderer.CompareTag(Accent_Tag))
                {
                    spriteRenderer.color = AccentTint;
                    continue;
                }

                if (spriteRenderer.CompareTag(Roof_Tag))
                {
                    spriteRenderer.color = RoofTint;
                    continue;
                }

                if (spriteRenderer.CompareTag(Door_Tag))
                    spriteRenderer.color = DoorTint;
            }
        }

        [ButtonGroup("Randomization")]
        [Button("Randomize All Colors", ButtonSizes.Large)]
        public void RandomizeAllColors()
        {
            WallTint = GenerateRandomColor();
            TrimTint = GenerateRandomColor();
            GlassTint = GenerateRandomColor();
            DoorTint = GenerateRandomColor();
            RoofTint = GenerateRandomColor();
            AccentTint = GenerateRandomColor();

            Refresh();
        }

        [ButtonGroup("Randomization")]
        [Button("Randomize Wall")]
        public void RandomizeWall() => RandomizeColor(ref WallTint);

        [ButtonGroup("Randomization")]
        [Button("Randomize Trim")]
        public void RandomizeTrim() => RandomizeColor(ref TrimTint);

        [ButtonGroup("Randomization")]
        [Button("Randomize Glass")]
        public void RandomizeGlass() => RandomizeColor(ref GlassTint);

        [ButtonGroup("Randomization")]
        [Button("Randomize Door")]
        public void RandomizeDoor() => RandomizeColor(ref DoorTint);

        [ButtonGroup("Randomization")]
        [Button("Randomize Roof")]
        public void RandomizeRoof() => RandomizeColor(ref RoofTint);

        [ButtonGroup("Randomization")]
        [Button("Randomize Accent")]
        public void RandomizeAccent() => RandomizeColor(ref AccentTint);

        [Button("Generate Color Palette", ButtonSizes.Medium)]
        public void GenerateColorPalette()
        {
            // Generate a cohesive color palette
            Color baseColor = GenerateRandomColor();

            WallTint = baseColor;
            TrimTint = GenerateComplementaryColor(baseColor);
            GlassTint = GenerateAnalogousColor(baseColor, 60f);
            DoorTint = GenerateTriadicColor(baseColor);
            RoofTint = GenerateAnalogousColor(baseColor, -60f);
            AccentTint = GenerateComplementaryColor(baseColor, 0.8f);

            Refresh();
        }

        private void RandomizeColor(ref Color colorToRandomize)
        {
            colorToRandomize = GenerateRandomColor();
            Refresh();
        }

        private Color GenerateRandomColor()
        {
            float hue;

            if (UseWarmColors)
            {
                // Warm colors: reds, oranges, yellows (0-60 degrees and 300-360 degrees)
                hue = Random.value < 0.5f ? Random.Range(0f, 60f) : Random.Range(300f, 360f);
            }
            else if (UseCoolColors)
            {
                // Cool colors: blues, greens, purples (120-300 degrees)
                hue = Random.Range(120f, 300f);
            }
            else
            {
                // Full spectrum
                hue = Random.Range(0f, 360f);
            }

            float saturation = Random.Range(MinSaturation, MaxSaturation);
            float brightness = Random.Range(MinBrightness, MaxBrightness);

            return Color.HSVToRGB(hue / 360f, saturation, brightness);
        }

        private Color GenerateComplementaryColor(Color baseColor, float intensity = 1f)
        {
            Color.RGBToHSV(baseColor, out float h, out float s, out float v);

            // Add 180 degrees for complementary color
            float complementaryHue = (h + 0.5f) % 1f;

            return Color.HSVToRGB(complementaryHue, s * intensity, v);
        }

        private Color GenerateAnalogousColor(Color baseColor, float hueOffset)
        {
            Color.RGBToHSV(baseColor, out float h, out float s, out float v);

            // Offset hue by specified degrees
            float analogousHue = (h + (hueOffset / 360f)) % 1f;
            if (analogousHue < 0) analogousHue += 1f;

            return Color.HSVToRGB(analogousHue, s, v);
        }

        private Color GenerateTriadicColor(Color baseColor)
        {
            Color.RGBToHSV(baseColor, out float h, out float s, out float v);

            // Add 120 degrees for triadic color
            float triadicHue = (h + (120f / 360f)) % 1f;

            return Color.HSVToRGB(triadicHue, s, v);
        }

        [Button("Reset to White")]
        public void ResetColors()
        {
            WallTint = Color.white;
            TrimTint = Color.white;
            GlassTint = Color.white;
            DoorTint = Color.white;
            RoofTint = Color.white;
            AccentTint = Color.white;

            Refresh();
        }
    }
}
