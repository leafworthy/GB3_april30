using TMPro;
using UnityEngine;

namespace __SCRIPTS.HUD_Displays
{
	public interface INeedPlayer
	{
		void SetPlayer(Player player);
	}
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
			LevelManager.OnStartLevel += (t) => UpdateDisplay();
			LevelManager.OnPlayerSpawned += (t) => UpdateDisplay();
		}

	
		public virtual void SetPlayer(Player player)
		{
			owner = player;
			PlayerStats.OnPlayerStatChange += Players_PlayerStatChange;
			PlayerStats.OnStatsReset += UpdateDisplay;
			UpdateDisplay();
		}

		private void Players_PlayerStatChange(Player player, PlayerStat playerStat)
		{
			UpdateDisplay();
		}

		private void UpdateDisplay()
		{
			if (owner == null) return;
			if(hasMax)
			{
				displayText.text = CurrentAmount.ToString() +"/" + PlayerStatsManager.I.MaxGas.ToString();
			}
			else
			{
				displayText.text = CurrentAmount.ToString();
			}
			var shaker = statIcon.gameObject.GetComponent<ObjectShaker>();
			if(shaker == null) shaker = statIcon.gameObject.AddComponent<ObjectShaker>();
			shaker.Shake(ObjectShaker.ShakeIntensityType.medium);
		}
	}
}