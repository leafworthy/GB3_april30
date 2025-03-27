using TMPro;
using UnityEngine;

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

	private float CurrentAmount => PlayerStatsManager.I.GetStatAmount(owner, statType);

	public virtual void SetPlayer(Player player)
	{
		Debug.Log("player assigned correctly");
		owner = player;
		PlayerStatsManager.I.OnPlayerStatChange += Players_PlayerStatChange;
		UpdateDisplay();
	}

	private void Players_PlayerStatChange(Player player, PlayerStat.StatType _statType, float newAmount)
	{
		if (player != owner) return;
		if (_statType != statType) return;
		UpdateDisplay();
	}

	private void UpdateDisplay()
	{
		displayText.text = CurrentAmount.ToString();
		var shaker = statIcon.gameObject.AddComponent<ObjectShaker>();
		shaker.Shake(ObjectShaker.ShakeIntensityType.medium);
	}
}