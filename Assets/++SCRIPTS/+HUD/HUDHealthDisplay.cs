

using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class HUDHealthDisplay : MonoBehaviour
{

	[FormerlySerializedAs("healthBar"),FormerlySerializedAs("bar")] public AmmoLifeBar lifeBar;
	public TMP_Text healthText;
	private Life playerDefence;
	public GameObject shakeIcon;

	public void SetPlayer(Player player)
	{
		if (playerDefence != null)
		{
			playerDefence.OnFractionChanged -= UpdateDisplay;
		}
		playerDefence = player.SpawnedPlayerGO.GetComponent<Life>();
		if (lifeBar.slowBarImage != null)
		{
			lifeBar.slowBarImage.color = player.color;
		}

		if (lifeBar.capSpriteRenderer != null)
		{
			lifeBar.capSpriteRenderer.color = player.color;
		}

		playerDefence.OnFractionChanged += UpdateDisplay;
		UpdateDisplay(0);
	}

	private void UpdateDisplay(float f)
	{
		healthText.text = Mathf.Ceil(playerDefence.Health).ToString();
		lifeBar.UpdateBar(playerDefence.Health, playerDefence.HealthMax);
		var shaker = shakeIcon.gameObject.AddComponent<ObjectShaker>();
		shaker.Shake(ObjectShaker.ShakeIntensityType.medium);
	}
}
