using System.Collections.Generic;
using Sirenix.OdinInspector;
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

		private void Update()
		{
			if(!Application.isPlaying) Refresh();
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
					continue;
				}

				spriteRenderer.color = Tint;
				toTint.Add(spriteRenderer);
			}
		}
	}
}
