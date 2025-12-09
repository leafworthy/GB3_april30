using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PalletteSwapper : MonoBehaviour
{
	public List<Material> pallettes = new();
	public SpriteRenderer spriteRenderer;
	public List<Material> characterPallettes = new();

	[Button]
	public void Swap()
	{
		spriteRenderer.material = characterPallettes[Random.Range(0, pallettes.Count)];
	}

	public void SetPallette(EnemySpawner.EnemyType type, int enemyTier)
	{
		characterPallettes = Services.assetManager.Players.GetCharacterPalettes(type);
		spriteRenderer.material = characterPallettes[enemyTier];
	}
}
