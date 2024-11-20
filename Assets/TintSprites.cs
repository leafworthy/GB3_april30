using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[ExecuteAlways]
public class TintSprites : MonoBehaviour
{
    
    public Color Tint = Color.white;
     public List< SpriteRenderer > toTint = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
	    Refresh();
    }

    // Update is called once per frame
    void Update()
    {
		foreach (var spriteRenderer in toTint)
		{
			spriteRenderer.color = Tint;
		}
    }

    [Button]
    public void Refresh()
    {
	   		toTint.Clear();
		var spriteRenderers = GetComponentsInChildren<SpriteRenderer>( true );
		foreach (var spriteRenderer in spriteRenderers)
		{
			if (spriteRenderer.CompareTag("dontcolor"))
			{
				spriteRenderer.color = Color.white;
				return;
			}
			toTint.Add(spriteRenderer);
		}
    }
}
