using System.Collections.Generic;
using UnityEngine;

namespace __SCRIPTS.UpgradeS
{
	public class PlayerUpgrades : MonoBehaviour
	{
		private Player player;
		[SerializeField]private List<Upgrade> upgrades = new();

		private void Start()
		{
			LevelManager.I.OnStartLevel += ResetUpgrades;
		}
		private void OnDisable()
		{
			LevelManager.I.OnStartLevel -= ResetUpgrades;
		}

		private void ResetUpgrades(GameLevel gameLevel)
		{
			foreach (var upgrade in upgrades)
			{
				upgrade.ResetUpgrade();
			}
			upgrades.Clear();
		}
		public bool BuyUpgrade(Upgrade upgrade)
		{
			if(upgrade.Cost > PlayerStatsManager.I.GetStatAmount(player,PlayerStat.StatType.TotalCash))
			{

				return false;
			}


			PlayerStatsManager.I.ChangeStat(player, PlayerStat.StatType.TotalCash, -upgrade.Cost);

				upgrade.UpgradeLevel();
			

			if (!upgrades.Contains(upgrade))
			{

				upgrades.Add(upgrade);
			}

			
			ApplyUpgrades(player);
			return true;
		}

		public void ApplyUpgrades(Player _player)
		{
			player = _player;
			foreach (var upgrade in upgrades)
			{

				upgrade.CauseEffect(player);
			}
		}

		public int GetTotalUpgradesPurchased()
		{
			var totalUpgrades = 0;
			foreach (var upgrade in upgrades)
			{
				totalUpgrades += upgrade.level;
			}

			return totalUpgrades;
		}
	}
}
