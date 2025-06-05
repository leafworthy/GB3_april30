using System.Collections.Generic;
using UnityEngine;

namespace __SCRIPTS
{
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

  

		public void Refresh()
		{
			toTint.Clear();
			var spriteRenderers = GetComponentsInChildren<SpriteRenderer>( true );
			foreach (var spriteRenderer in spriteRenderers)
			{
				if (spriteRenderer.CompareTag("dontcolor"))
				{
					spriteRenderer.color = Color.white;
					continue;
				}
				toTint.Add(spriteRenderer);
			}
		}
	}
}
