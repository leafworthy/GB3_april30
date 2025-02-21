using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[ExecuteInEditMode]
public class HUDHealthDisplay : MonoBehaviour
{

	[FormerlySerializedAs("bar")] public Bar_FX barFX;
	public TMP_Text healthText;
	public TMP_Text MaxHealthText;
	private Life playerDefence;
	public GameObject shakeIcon;
	public Image capSpriteRenderer;

	public void SetPlayer(Player player)
	{
		if (playerDefence != null)
		{
			playerDefence.OnFractionChanged -= UpdateDisplay;
		}
		playerDefence = player.SpawnedPlayerGO.GetComponentInChildren<Life>();
		if (barFX.slowBarImage != null)
		{
			barFX.slowBarImage.color = player.color;
		}

		if (capSpriteRenderer != null)
		{
			capSpriteRenderer.color = player.color;
		}

		playerDefence.OnFractionChanged += UpdateDisplay;
		UpdateDisplay(0);
	}



	private void UpdateDisplay(float f)
	{
		healthText.text = Mathf.Ceil(playerDefence.Health).ToString();
		MaxHealthText.text = Mathf.Ceil(playerDefence.HealthMax).ToString();
		barFX.UpdateBar(playerDefence.Health, playerDefence.HealthMax);
		var shaker = shakeIcon.gameObject.AddComponent<ObjectShaker>();
		shaker.Shake(ObjectShaker.ShakeIntensityType.medium);
	}
}