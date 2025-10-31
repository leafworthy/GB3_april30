using UnityEngine;

namespace __SCRIPTS
{
	public class EnemyTier : MonoBehaviour
	{

		[SerializeField] private int enemyTier;
		private BasicHealth health => _health ??= GetComponent<BasicHealth>();
		private BasicHealth _health;

		public int GetEnemyTier() => enemyTier;

		public void SetEnemyTier(int tier)
		{
			enemyTier = tier;
			health.FillHealth();
			var paletteSwapper = GetComponent<PalletteSwapper>();
			paletteSwapper?.SetPallette(tier);
		}

	}
}
