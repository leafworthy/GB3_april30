using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PalletteSwapper : MonoBehaviour
{
	public List<Material> pallettes = new();
	public SpriteRenderer spriteRenderer;

	[Button]
	public void Swap()
	{
		spriteRenderer.material = pallettes[Random.Range(0, pallettes.Count)];
	}

	public void SetPallette(int index)
	{
		Debug.Log("pallette set to " + index);
		if (index < 0 || index >= pallettes.Count) return;
		spriteRenderer.material = pallettes[index];
	}
}
