using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpawnGridCell : MonoBehaviour
{
	public SpriteRenderer Avatar;
	public TextMeshProUGUI TierText;
	public TextMeshProUGUI AmountText;

	private PalletteSwapper palletteSwapper => _palletteSwapper ??= GetComponent<PalletteSwapper>();
	private PalletteSwapper _palletteSwapper;


	public void Set(Sprite avatar, EnemySpawner.EnemyType enemyType,  int tier, int amount)
	{
		Avatar.sprite = avatar;
		TierText.text =  tier.ToString();
		AmountText.text = "x" + amount;
		palletteSwapper.SetPallette(enemyType,tier);
	}
}
