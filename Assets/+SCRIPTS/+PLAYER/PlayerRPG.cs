using __SCRIPTS;
using UnityEngine;

public class PlayerRPG : MonoBehaviour
{
	public int Level = 1;
	Life _player_life;
	float extraHealthFactorPerLevel = 1;
	float extraDamageFactorPerLevel = 1;
	float extraSpeedFactorPerLevel = 1;
	PlayerStats _playerStats;
	public int[] LevelTresholds = {0, 100, 300, 600, 1000, 1500, 2100, 2800, 3600, 4500};

	Player _player;
	void Start()
	{
		_playerStats = GetComponent<PlayerStats>();
		_playerStats.OnPlayerStatChange += PlayerStats_OnPlayerStatChange;
		_player = GetComponent<Player>();
		_player.OnPlayerSpawned += Player_OnPlayerSpawned;
	}

	void Player_OnPlayerSpawned()
	{
		_player_life = _player.SpawnedPlayerGO.GetComponent<Life>();
		ResetLevel();
	}

	void PlayerStats_OnPlayerStatChange(Player player, PlayerStat stat)
	{
		if (stat.type != PlayerStat.StatType.Experience) return;
		PlayerGainsExperience(stat.GetStatAmount());
	}

	void PlayerGainsExperience(float experiencePoints)
	{
		Debug.Log("Player " + _playerStats.name + " gained " + experiencePoints + " XP.");
		_playerStats.ChangeStat(PlayerStat.StatType.Experience, experiencePoints);

		var correctLevel = GetLevelForExperience();
		if (correctLevel <= Level) return;
		Level++;
		Level = correctLevel;
		_playerStats.SetStatValue(PlayerStat.StatType.Level,Level);
		SetUnitStatsForLevel();
		Debug.Log("Player " + _playerStats.name + " leveled up to level " + Level);
	}

	int GetLevelForExperience()
	{
		var experience = _playerStats.GetStatValue(PlayerStat.StatType.Experience);
		for (var i = 0; i < LevelTresholds.Length; i++)
			if (experience < LevelTresholds[i])
				return i;

		return 0;
	}

	void ResetLevel()
	{
		Level = 1;
		Debug.Log("Player " + _playerStats.name + " level reset to " + Level);
		SetUnitStatsForLevel();
	}

	void SetUnitStatsForLevel()
	{
		if (_player_life == null) return;
		_player_life.Stats.ExtraHealthFactor = (Level - 1) * extraHealthFactorPerLevel;
		_player_life.Stats.ExtraDamageFactor = (Level - 1) * extraDamageFactorPerLevel;
		_player_life.Stats.ExtraSpeedFactor = (Level - 1) * extraSpeedFactorPerLevel;
	}
}
