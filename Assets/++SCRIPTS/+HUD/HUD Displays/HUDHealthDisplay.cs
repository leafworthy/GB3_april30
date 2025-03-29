using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace __SCRIPTS.HUD_Displays
{
	[ExecuteInEditMode]
	public class HUDHealthDisplay : MonoBehaviour, INeedPlayer {

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
				barFX.slowBarImage.color = player.playerColor;
			}

			if (capSpriteRenderer != null)
			{
				capSpriteRenderer.color = player.playerColor;
			}

			playerDefence.OnFractionChanged += UpdateDisplay;
			barFX = GetComponentInChildren<Bar_FX>();
			UpdateDisplay(0);
		}



		private void UpdateDisplay(float f)
		{
			healthText.text = Mathf.Ceil(playerDefence.Health).ToString();
			MaxHealthText.text = "/" + Mathf.Ceil(playerDefence.HealthMax).ToString();
			barFX.UpdateBar(playerDefence.Health, playerDefence.HealthMax);
			var shaker = shakeIcon.gameObject.AddComponent<ObjectShaker>();
			shaker.Shake(ObjectShaker.ShakeIntensityType.medium);
		}
	}
}