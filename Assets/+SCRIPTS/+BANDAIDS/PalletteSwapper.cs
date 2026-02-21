using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PalletteSwapper : MonoBehaviour
{
	public SpriteRenderer spriteRenderer;
	public List<Material> characterPallettes = new();



	public void SetPallette(EnemySpawner.EnemyType type, int enemyTier)
	{
		characterPallettes = Services.assetManager.Players.GetCharacterPalettes(type);
		spriteRenderer.material = characterPallettes[enemyTier];
	}
}
