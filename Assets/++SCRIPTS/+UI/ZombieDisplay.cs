using TMPro;
using UnityEngine;

[ExecuteInEditMode]
public class ZombieDisplay : MonoBehaviour
{
	public AmmoBar bar;
	public TMP_Text killText;
	public GameObject shakeIcon;
	private Player currentPlayer;
	private int totalKills;
	private int originalTotalEnemies;

	public void SetPlayer(Player player)
	{
		totalKills = 0;
		currentPlayer = player;
		ENEMIES.OnEnemyKilled += EnemyKilled;
		originalTotalEnemies = ENEMIES.GetNumberOfLivingEnemies();
		UpdateDisplay();
		GAME.OnGameEnd += CleanUp;
	}

	private void CleanUp()
	{
		ENEMIES.OnEnemyKilled -= EnemyKilled;
		GAME.OnGameEnd -= CleanUp;
	}

	private void EnemyKilled(IPlayerAttackHandler killer)
	{
		if (killer.GetPlayer() != currentPlayer) return;
		totalKills++;
		UpdateDisplay();
	}

	private void UpdateDisplay()
	{
		killText.text = totalKills.ToString();
		bar.UpdateBar(totalKills, originalTotalEnemies);
		var shaker = shakeIcon.gameObject.AddComponent<ObjectShaker>();
		shaker.Shake(ObjectShaker.ShakeIntensityType.medium);
	}
}
