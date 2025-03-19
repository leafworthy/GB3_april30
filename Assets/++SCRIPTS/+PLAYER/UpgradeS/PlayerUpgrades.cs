using System.Collections.Generic;
using UnityEngine;

public class PlayerUpgrades : MonoBehaviour
{
	private Player player;
	[SerializeField]private List<Upgrade> upgrades = new();



	public bool BuyUpgrade(Upgrade upgrade)
	{
		if(upgrade.GetCost() > PlayerStatsManager.I.GetStatAmount(player,PlayerStat.StatType.TotalCash))
		{
			Debug.Log("not enough cash for " + upgrade.GetName());
			return false;
		}

		Debug.Log("upgrade purchased: " + upgrade.GetName());
		if(upgrades.Contains(upgrade))
		{
			upgrade.UpgradeLevel();
		}
		else
		{
			upgrades.Add(upgrade);
		}

		PlayerStatsManager.I.ChangeStat(player,PlayerStat.StatType.TotalCash, -upgrade.GetCost());
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

}
