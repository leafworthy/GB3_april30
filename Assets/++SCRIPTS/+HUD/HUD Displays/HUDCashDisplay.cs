using TMPro;
using UnityEngine;

public class HUDCashDisplay : MonoBehaviour
{
	public TMP_Text cashText;
	private Life playerDefence;
	public GameObject shakeIcon;
	private int totalCash;
	private Player owner;

	public void SetPlayer(Player player)
	{
		totalCash = (int) PlayerStatsManager.I.GetStatAmount(player, PlayerStat.StatType.TotalCash);
		owner = player;
		PlayerStatsManager.I.OnPlayerStatChange += PlayerStatChange;
		UpdateDisplay();
	}

	private void PlayerStatChange(Player player, PlayerStat.StatType statType, float newAmount)
	{
		if (player != owner) return;
		if (statType is not PlayerStat.StatType.TotalCash) return;
		totalCash = (int) newAmount;
		UpdateDisplay();
	}


	private void UpdateDisplay()
	{
		cashText.text = "$"+totalCash.ToString();
		var shaker = shakeIcon.gameObject.AddComponent<ObjectShaker>();
		shaker.Shake(ObjectShaker.ShakeIntensityType.medium);
	}
}