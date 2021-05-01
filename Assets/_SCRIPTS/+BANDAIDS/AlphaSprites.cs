using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace _SCRIPTS
{
	[ExecuteInEditMode]
	public class AlphaSprites : MonoBehaviour
	{
		[SerializeField] private float currentAlpha = 1;
		[SerializeField] private List<SpriteRenderer> sprites = new List<SpriteRenderer>();
		public bool updateAlpha = false;

		private void Start()
		{

			updateAlphaSprites();
		}

		public void SetAlpha(float newAlpha)
		{
			currentAlpha = newAlpha;
			updateAlphaSprites();
			int voovoo = 26;

		}



		[Button()]
		private void GetSprites()
		{
			sprites.Clear();
			foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
			{
				sprites.Add(sr);
			}
		}

		private void Update()
		{
			if (updateAlpha)
			{
				updateAlphaSprites();
			}
		}

		private void updateAlphaSprites()
		{

			GetSprites();


			foreach (SpriteRenderer spriteRenderer in sprites)
			{
				if (!spriteRenderer.CompareTag("dontalpha"))
				{
					var col = spriteRenderer.color;
					col.a = currentAlpha;
					spriteRenderer.color = col;
				}
			}
		}
	}
}
