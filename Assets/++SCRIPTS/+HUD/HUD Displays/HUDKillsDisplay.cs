using TMPro;
using UnityEngine;

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