using TMPro;
using UnityEngine;

public class HUDGasDisplay : MonoBehaviour
{
	public TMP_Text gasText;
	public GameObject shakeIcon;
	private Player owner;
	private float totalGas;

	public void SetPlayer(Player player)
	{
		totalGas = player.GetPlayerStatAmount(PlayerStat.StatType.Gas);
		owner = player;
		PlayerStatsHandler.OnPlayerStatChange += Players_PlayerStatChange;
		UpdateDisplay();
	}


	private void Players_PlayerStatChange(Player player, PlayerStat stat)
	{
		if (player != owner) return;
		if (stat.type != PlayerStat.StatType.Gas) return;
		totalGas = stat.value;
		
		UpdateDisplay();
	}

	private void UpdateDisplay()
	{
		gasText.text = totalGas.ToString();
		var shaker = shakeIcon.gameObject.AddComponent<ObjectShaker>();
		shaker.Shake(ObjectShaker.ShakeIntensityType.medium);
	}
}