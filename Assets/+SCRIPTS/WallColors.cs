using __SCRIPTS;
using UnityEngine;

[ExecuteInEditMode]
public class WallColors : MonoBehaviour
{
	public Color wallColor = Color.white;
	public Color trimColor = Color.white;
	public Color glassColor = Color.white;
	public TintSprites WallColorTintSprite;
	public TintSprites TrimColorTintSprite;
	public TintSprites GlassColorTintSprite;

	private void OnEnable()
	{
		Refresh();
	}

	private void Update()
	{
		Refresh();
	}

	private void Refresh()
	{
		if (WallColorTintSprite != null)
		{
			WallColorTintSprite.Tint = wallColor;
		}

		if (TrimColorTintSprite != null)
		{
			TrimColorTintSprite.Tint = trimColor;
		}

		if (GlassColorTintSprite != null)
		{
			GlassColorTintSprite.Tint = glassColor;
		}
	}
}
