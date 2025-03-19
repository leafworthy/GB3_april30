using TMPro;
using UnityEngine;

public interface INeedPlayer
{
	void SetPlayer(Player player);
}
public class HUDStatDisplay : MonoBehaviour, INeedPlayer
{
	public PlayerStat.StatType statType {  get; set; }
	public TMP_Text displayText;
	public GameObject statIcon;
	private Player owner;
	private float currentAmount;

	public void SetPlayer(Player player)
	{
		currentAmount = PlayerStatsManager.I.GetStatAmount(player, statType);
		owner = player;
		PlayerStatsManager.I.OnPlayerStatChange += Players_PlayerStatChange;
		UpdateDisplay();
	}

	private void Players_PlayerStatChange(Player player, PlayerStat.StatType _statType, float newAmount)
	{
		if (player != owner) return;
		if (_statType != statType) return;
		currentAmount = newAmount;

		UpdateDisplay();
	}

	private void UpdateDisplay()
	{
		displayText.text = currentAmount.ToString();
		var shaker = statIcon.gameObject.AddComponent<ObjectShaker>();
		shaker.Shake(ObjectShaker.ShakeIntensityType.medium);
	}
}