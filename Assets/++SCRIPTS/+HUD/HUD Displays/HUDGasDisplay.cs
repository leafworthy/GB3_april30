using TMPro;
using UnityEngine;

public class HUDGasDisplay : HUDStatDisplay
{
	public TMP_Text gasText;
	public GameObject shakeIcon;
	private Player owner;
	private float totalGas;
	private PlayerStat.StatType statType1;

	public void SetPlayer(Player player)
	{
		totalGas = PlayerStatsManager.I.GetStatAmount(player,PlayerStat.StatType.Gas);
		owner = player;
		PlayerStatsManager.I.OnPlayerStatChange += Players_PlayerStatChange;
		UpdateDisplay();
	}


	private void Players_PlayerStatChange(Player player, PlayerStat.StatType statType, float newAmount)
	{
		if (player != owner) return;
		if (statType != PlayerStat.StatType.Gas) return;
		totalGas = newAmount;
		
		UpdateDisplay();
	}

	private void UpdateDisplay()
	{
		gasText.text = totalGas.ToString();
		var shaker = shakeIcon.gameObject.AddComponent<ObjectShaker>();
		shaker.Shake(ObjectShaker.ShakeIntensityType.medium);
	}
}