using _SCRIPTS;
using TMPro;
using UnityEngine;

[ExecuteInEditMode]
public class HealthDisplay : MonoBehaviour
{
	public AmmoBar bar;
	public TMP_Text healthText;
	private DefenceHandler playerDefence;
	public GameObject shakeIcon;

	public void SetPlayer(Player player)
	{
		if (playerDefence != null)
		{
			playerDefence.OnFractionChanged -= UpdateDisplay;
		}
		playerDefence = player.SpawnedPlayerGO.GetComponent<DefenceHandler>();

		bar.fastBarImage.color = player.playerColor;
		bar.capSpriteRenderer.color = player.playerColor;
		playerDefence.OnFractionChanged += UpdateDisplay;
		UpdateDisplay(0);
	}

	private void UpdateDisplay(float f)
	{
		healthText.text = Mathf.Ceil(playerDefence.Health).ToString();
		bar.UpdateBar(playerDefence.Health, playerDefence.HealthMax);
		var shaker = shakeIcon.gameObject.AddComponent<ObjectShaker>();
		shaker.Shake(ObjectShaker.ShakeIntensityType.low);
	}
}
