using _SCRIPTS;
using TMPro;
using UnityEngine;

[ExecuteInEditMode]
public class ZombieDisplay : MonoBehaviour
{
	public AmmoBar bar;
	public TMP_Text killText;
	private DefenceHandler playerDefence;
	public GameObject shakeIcon;
	private Player currentPlayer;
	private int totalKills;
	private int originalTotalEnemies;

	public void SetPlayer(Player player)
	{
		totalKills = 0;
		currentPlayer = player;
		player.OnKillEnemy += EnemyKilled;
		originalTotalEnemies = ENEMIES.GetNumberOfLivingEnemies();
		UpdateDisplay();
	}

	private void EnemyKilled(Player killer)
	{
		if (killer == currentPlayer)
		{
			totalKills++;
			UpdateDisplay();
		}
	}

	private void UpdateDisplay()
	{
		killText.text = totalKills.ToString();
		bar.UpdateBar(totalKills, originalTotalEnemies);
		var shaker = shakeIcon.gameObject.AddComponent<ObjectShaker>();
		shaker.Shake(ObjectShaker.ShakeIntensityType.medium);
	}
}
