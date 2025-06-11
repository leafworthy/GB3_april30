using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using VInspector;

namespace __SCRIPTS
{
	[ExecuteAlways]
	public class ColorAllSprites : MonoBehaviour
	{
		public HouseColorScheme.ColorType color;
		private List<SpriteRenderer> spriteRenderer;
		private HouseColorScheme houseColorScheme;
		private SpriteShapeRenderer shapeRenderer;
		private Tilemap tileMap;
#if UNITY_EDITOR
		public void Update()
		{
			if (!Application.isPlaying)
			{
				// Only update in edit mode when necessary and limit frequency
				if (Time.realtimeSinceStartup - lastUpdateTime > 0.5f)
				{
					TintAll();
					lastUpdateTime = Time.realtimeSinceStartup;
				}
			}
		}


		private float lastUpdateTime = 0f;
#endif
		private void OnEnable()
		{
			TintAll();
		}

		private void GetAllSprites()
		{
			spriteRenderer = GetComponentsInChildren<SpriteRenderer>().ToList();
			houseColorScheme = GetComponentInParent<ColorSchemeHandler>().houseColors;
			tileMap = GetComponent<Tilemap>();
		}


		[Button]
		private void TintAll()
		{
			GetAllSprites();
			shapeRenderer = GetComponent<SpriteShapeRenderer>();
			if (shapeRenderer != null) shapeRenderer.color = houseColorScheme.GetColor(color);
			if (tileMap != null) tileMap.color = houseColorScheme.GetColor(color);

			foreach (var sr in spriteRenderer)
			{
				if (sr.gameObject.CompareTag("dontcolor")) continue;
				sr.color = houseColorScheme.GetColor(color);
			}

		}
	}
}
