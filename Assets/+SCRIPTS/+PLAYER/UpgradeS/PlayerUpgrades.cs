using System.Collections.Generic;
using UnityEngine;

namespace __SCRIPTS.UpgradeS
{
	public class PlayerUpgrades : ServiceUser
	{
		private Player player;
		[SerializeField]private List<Upgrade> upgrades = new();
		private void Start()
		{
			levelManager.OnStartLevel += ResetUpgrades;
		}
		private void OnDisable()
		{
			levelManager.OnStartLevel -= ResetUpgrades;
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
			if(upgrade.Cost > playerStatsManager.GetStatAmount(player,PlayerStat.StatType.TotalCash))
			{

				return false;
			}


			playerStatsManager.ChangeStat(player, PlayerStat.StatType.TotalCash, -upgrade.Cost);

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
