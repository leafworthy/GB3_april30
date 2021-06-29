using TMPro;
using UnityEngine;

public class CashDisplay : MonoBehaviour
{
	public TMP_Text cashText;
	private DefenceHandler playerDefence;
	public GameObject shakeIcon;
	private Player currentPlayer;
	private int totalCash;

	public void SetPlayer(Player player)
	{
		totalCash = 0;
		currentPlayer = player;
		var stats = player.SpawnedPlayerGO.GetComponent<UnitStats>();
		if (stats != null)
		{
			stats.OnStatChange += OnStatChange;
		}
		UpdateDisplay();
	}

	private void OnStatChange(UnitStat obj)
	{
		if (obj.type is StatType.cash)
		{
			totalCash = (int)obj.value;
			UpdateDisplay();
		}
	}


	private void UpdateDisplay()
	{
		cashText.text = "$"+totalCash.ToString();
		var shaker = shakeIcon.gameObject.AddComponent<ObjectShaker>();
		shaker.Shake(ObjectShaker.ShakeIntensityType.medium);
	}
}
