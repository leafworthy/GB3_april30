using System.Collections.Generic;
using UnityEngine;

namespace GangstaBean.Player.Upgrades.UpgradeS
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
				Debug.Log("not enough cash for " + upgrade.GetName());
				return false;
			}

			Debug.Log("upgrade purchased: " + upgrade.GetName());
			PlayerStatsManager.I.ChangeStat(player, PlayerStat.StatType.TotalCash, -upgrade.Cost);
				Debug.Log("upgrade level up");
				upgrade.UpgradeLevel();
			

			if (!upgrades.Contains(upgrade))
			{
				Debug.Log("add new upgrade " + upgrade.GetName());
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
				Debug.Log("Applying upgrade" + upgrade.GetName());
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
