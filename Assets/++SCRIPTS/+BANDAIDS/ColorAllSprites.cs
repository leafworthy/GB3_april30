using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

public class ColorAllSprites : MonoBehaviour
{
	public HouseColorScheme.ColorType color;
	private List<SpriteRenderer> spriteRenderer;
	private HouseColorScheme houseColorScheme;
	private SpriteShapeRenderer shapeRenderer;

	public void Update()
	{
		if (!Application.isPlaying) TintAll();
	}

	private void OnEnable()
	{
		
		TintAll();
	}

	private void GetAllSprites()
	{
		spriteRenderer = GetComponentsInChildren<SpriteRenderer>().ToList();
		houseColorScheme = GetComponentInParent<ColorSchemeHandler>().houseColors;
	}


	private void TintAll()
	{
		GetAllSprites();
		shapeRenderer = GetComponent<SpriteShapeRenderer>();
		if (shapeRenderer != null) shapeRenderer.color = houseColorScheme.GetColor(color);
		
		foreach (var sr in spriteRenderer)
		{
			if (sr.gameObject.CompareTag("dontcolor")) continue;
			sr.color = houseColorScheme.GetColor(color);
		}

	}
}