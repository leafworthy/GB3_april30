using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace _SCRIPTS
{
	[ExecuteInEditMode]
	public class BlackoutSprites : MonoBehaviour
	{
		[SerializeField] private Dictionary<SpriteRenderer, Color> sprites = new Dictionary<SpriteRenderer, Color>();
		public bool blackOut;

		private void Start()
		{
			SetOriginalColors();
		}

		[Button()]
		private void SetOriginalColors()
		{
			sprites.Clear();
			foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
			{
				sprites.Add(sr, sr.color);
			}
		}


		[Button()]
		public void ResetOriginalColors()
		{
			foreach (KeyValuePair<SpriteRenderer, Color> sr in sprites)
			{
				var col = sr.Value;
				col.a = sr.Key.color.a;
				sr.Key.color = sr.Value;
			}
		}

		[Button()]
		public void BlackOut()
		{
			foreach (KeyValuePair<SpriteRenderer, Color> sr in sprites)
			{
				sr.Key.color = new Color(0, 0, 0, sr.Key.color.a);
			}
		}
	}
}
