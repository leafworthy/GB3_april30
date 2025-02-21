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
		totalCash = (int) player.GetPlayerStatAmount(PlayerStat.StatType.TotalCash);
		owner = player;
		PlayerStatsHandler.OnPlayerStatChange += PlayerStatChange;
		UpdateDisplay();
	}

	private void PlayerStatChange(Player player, PlayerStat unitStat)
	{
		if (player != owner) return;
		if (unitStat.type is not PlayerStat.StatType.TotalCash) return;
		totalCash = (int) unitStat.value;
		UpdateDisplay();
	}


	private void UpdateDisplay()
	{
		cashText.text = "$"+totalCash.ToString();
		var shaker = shakeIcon.gameObject.AddComponent<ObjectShaker>();
		shaker.Shake(ObjectShaker.ShakeIntensityType.medium);
	}
}