using System.Collections.Generic;
using GangstaBean.Core;
using UnityEngine;
using VInspector;

[ExecuteAlways]
public class PalletteSwapper : MonoBehaviour, IPoolable
{
public List<Material> pallettes = new List<Material>();
public SpriteRenderer spriteRenderer;

    public void OnPoolSpawn()
    {
        spriteRenderer.material = pallettes[Random.Range(0, pallettes.Count)];
    }

    public void SetPallette(Material mat)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.material = mat;
        }
    }

    [Button]
    public void Swap()
    {
        spriteRenderer.material = pallettes[Random.Range(0, pallettes.Count)];
    }
    public void OnPoolDespawn()
    {
    }
}
