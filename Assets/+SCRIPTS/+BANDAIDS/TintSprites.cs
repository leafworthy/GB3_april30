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
		public bool autoRefresh = true;
		// Start is called once before the first execution of Update after the MonoBehaviour is created
		void OnEnable()
		{
			Refresh();
		}

		private void Update()
		{
			if(!Application.isPlaying && autoRefresh) Refresh();
		}

		[Button]
		private void Refresh()
		{
			foreach (var spriteRenderer in toTint)
			{
				if (spriteRenderer.CompareTag("dontcolor"))
				{
					spriteRenderer.color = Color.white;
					continue;
				}

				spriteRenderer.color = Tint;
			}
		}

		[Button]
		public void GatherRenderers()
		{
			toTint.Clear();
			var spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
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
