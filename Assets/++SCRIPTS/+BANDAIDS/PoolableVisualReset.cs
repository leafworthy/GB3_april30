using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GangstaBean.Core;

namespace __SCRIPTS
{
    /// <summary>
    /// Generic component that resets visual properties when objects are spawned from the pool
    /// Attach this to any pooled object that needs visual state reset
    /// </summary>
    public class PoolableVisualReset : MonoBehaviour, IPoolable
    {
        [Header("Reset Options")]
        [SerializeField] private bool resetSpriteRendererColors = true;
        [SerializeField] private bool resetMaterialTints = true;
        [SerializeField] private Color defaultSpriteColor = Color.white;

        private List<SpriteRenderer> spriteRenderers = new();
        private List<Renderer> renderers = new();
        private static readonly int TintProperty = Shader.PropertyToID("_Tint");

        private void Awake()
        {
            CacheComponents();
        }

        private void CacheComponents()
        {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true).ToList();
            renderers = GetComponentsInChildren<Renderer>(true).ToList();
        }

        public void OnPoolSpawn()
        {
            // Refresh component cache in case hierarchy changed
            CacheComponents();

            if (resetSpriteRendererColors)
            {
                ResetSpriteRendererColors();
            }

            if (resetMaterialTints)
            {
                ResetMaterialTints();
            }
        }

        public void OnPoolDespawn()
        {
            // Nothing needed when despawning
        }

        private void ResetSpriteRendererColors()
        {
            foreach (var spriteRenderer in spriteRenderers)
            {
                if (spriteRenderer != null && !spriteRenderer.CompareTag("dontcolor"))
                {
                    spriteRenderer.color = defaultSpriteColor;
                }
            }
        }

        private void ResetMaterialTints()
        {
            Color transparentTint = new Color(1, 1, 1, 1);
            foreach (var renderer in renderers)
            {
                if (renderer != null && renderer.material != null)
                {
                    if (renderer.material.HasProperty(TintProperty))
                    {
                        renderer.material.SetColor(TintProperty, transparentTint);
                    }
                }
            }
        }
    }
}
