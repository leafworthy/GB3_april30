using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[ExecuteInEditMode]
public class AlphaSprites : MonoBehaviour
{
	[SerializeField] private float currentAlpha = 1;
	[SerializeField] private List<SpriteRenderer> sprites = new List<SpriteRenderer>();
	public bool updateAlpha;
	private string dontalpha = "dontalpha";

	private void Start()
	{

		updateAlphaSprites();
	}

	public void SetAlpha(float newAlpha)
	{
		currentAlpha = newAlpha;
		updateAlphaSprites();
	}



	[Button()]
	private void GetSprites()
	{
		sprites.Clear();
		foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
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
			if (spriteRenderer.CompareTag(dontalpha)) continue;
			var col = spriteRenderer.color;
			col.a = currentAlpha;
			spriteRenderer.color = col;
		}
	}
}
