using __SCRIPTS._BANDAIDS;
using __SCRIPTS._COMMON;
using __SCRIPTS._PLAYER;
using __SCRIPTS._UNITS;
using TMPro;
using UnityEngine;

namespace __SCRIPTS._HUD
{
	[ExecuteInEditMode]
	public class HUDKillsDisplay : MonoBehaviour
	{
		public TMP_Text killText;
		public GameObject shakeIcon;
		private Player owner;
		private int totalKills;

		public void SetPlayer(Player player)
		{
			totalKills = 0;
			owner = player;
			EnemyManager.OnPlayerKillsEnemy += EnemiesOnPlayerKillsEnemy;
			UpdateDisplay();
		}

		private void EnemiesOnPlayerKillsEnemy(Player killer, Life life)
		{
			if (killer != owner) return;
			totalKills++;
			UpdateDisplay();
		}

		private void UpdateDisplay()
		{
			killText.text = totalKills.ToString();
			var shaker = shakeIcon.gameObject.AddComponent<ObjectShaker>();
			shaker.Shake(ObjectShaker.ShakeIntensityType.medium);
		}
	}
}