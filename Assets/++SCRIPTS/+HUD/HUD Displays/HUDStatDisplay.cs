using GangstaBean.Core;
using TMPro;
using UnityEngine;

namespace __SCRIPTS.HUD_Displays
{

	public class HUDStatDisplay : MonoBehaviour, INeedPlayer
	{
		public PlayerStat.StatType statType;
		public TMP_Text displayText;
		public GameObject statIcon;
		private Player owner;
		public bool hasMax;

		private float CurrentAmount => PlayerStatsManager.I.GetStatAmount(owner, statType);

		private void Start()
		{
			LevelManager.I.OnStartLevel += (t) => UpdateDisplay();
			LevelManager.I.OnPlayerSpawned += (t) => UpdateDisplay();
		}


		public virtual void SetPlayer(Player _player)
		{
			owner = _player;
			var playerStats = owner.GetComponent<PlayerStats>();
			if(playerStats == null)
			{

				return;
			}

			playerStats.OnPlayerStatChange += Players_PlayerStatChange;
			playerStats.OnStatsReset += UpdateDisplay;
			UpdateDisplay();
		}

		private void Players_PlayerStatChange(Player player, PlayerStat playerStat)
		{
			UpdateDisplay();
		}

		private void UpdateDisplay()
		{
			if (owner == null) return;
			
			var currentAmount = CurrentAmount;
			// Don't display negative values (which indicate uninitialized stats)
			if (currentAmount < 0) currentAmount = 0;
			
			if(hasMax)
			{
				displayText.text = currentAmount.ToString() +"/" + PlayerStatsManager.I.MaxGas.ToString();
			}
			else
			{
				displayText.text = currentAmount.ToString();
			}

		}
	}
}
